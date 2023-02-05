using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Updater.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace FPLV2.Updater.Functions;

public class UpdateFunction : Function
{
    public FplClient FplClient { get; init; }

    public UpdateFunction(ILoggerFactory loggerFactory, IConfiguration configuration, FplClient fplClient, IUnitOfWork unitOfWork) : base(loggerFactory, configuration, fplClient, unitOfWork)
    {
        Logger = loggerFactory.CreateLogger<UpdateFunction>();
        FplClient = fplClient;
    }

    [Function("UpdateFunction")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] FunctionInfo info)
    {
        Logger.LogInformation($"UpdateFunction executed at: {DateTime.Now}");

        try
        {
            var result = await FplClient.GetBootstrapStatic();
            if (result == null)
                return;

            var currentGameweek = result.Gameweeks.FirstOrDefault(x => x.IsCurrent);
            if (currentGameweek == null)
                return;

            // update seasons
            var seasonId = await UpdateSeasons(result);
            if (seasonId == 0)
                return;

            // update teams
            var updatedTeams = await UpdateTeams(result.Teams ?? new Team[] { }, seasonId);
            if (updatedTeams == false)
                return;

            // update elements
            var updatedElements = await UpdateElements(result.Elements ?? new Element[] { }, seasonId);
            if (updatedElements == false)
                return;

            var dbElementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
            var dbElements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
            for (var i = 1; i <= currentGameweek.Id; i++)
            {
                // update element stats
                var elementStats = await FplClient.GetElementStats(i);
                await UpdateElementStats(elementStats, dbElementStats, dbElements, i);
            }

            // loop leagues that need to be updated
            var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(seasonId);
            foreach (var l in leagues)
            {
                // get league standings
                var standings = await FplClient.GetLeagueStandings(l.LeagueId);
                if (standings == null)
                    continue;

                // league type must be 'x', which are the custom created leagues
                if (standings.League.LeagueType != "x")
                    continue;

                // update league
                l.Name = standings.League.Name;
                var updatedLeague = await UnitOfWork.Leagues.Update(l);
                if (updatedLeague == false)
                    continue;

                // update players in league
                await UpdatePlayers(standings.Results.Players, l.LeagueId);

                foreach (var p in standings.Results.Players)
                {
                    for (var i = 1; i <= currentGameweek.Id; i++)
                    {
                        // update picks
                        var picks = await FplClient.GetPicks(p.Entry, i);
                        await UpdatePicks(picks, i, l.LeagueId, p.Entry);
                    }

                    // update points
                    var points = await FplClient.GetPointsHistory(p.Entry);
                    await UpdatePoints(points, l.LeagueId, p.Entry);
                }
            }

        }
        catch (Exception ex)
        {

        }
     }

    private async Task<int> UpdateSeasons(BootstrapStatic result)
    {
        var openingDate = result.Gameweeks?.FirstOrDefault()?.DeadlineTime ?? DateTime.MinValue;
        var finalDate = result.Gameweeks?.LastOrDefault()?.DeadlineTime ?? DateTime.MinValue;
        if (openingDate == DateTime.MinValue || finalDate == DateTime.MinValue)
            return 0;

        var seasonYear = $"{openingDate.Year}/{finalDate.Year.ToString().Substring(2, 2)}"; // e.g. 2020/21
        var dbSeasons = await UnitOfWork.Seasons.GetAll();

        var seasonId = dbSeasons.FirstOrDefault(x => x.Year == seasonYear)?.Id ?? 0;
        if (seasonId == 0)
            seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = seasonYear });

        return seasonId;
    }

    private async Task<bool> UpdateTeams(Team[] teams, int seasonId)
    {
        var dbTeams = await UnitOfWork.Teams.GetAllBySeasonId(seasonId);

        foreach (var t in teams)
        {
            var team = (Database.Models.Team)t;
            team.SeasonId = seasonId;

            var dbTeam = dbTeams.FirstOrDefault(x => x.TeamId == t.Id);
            if (dbTeam == null)
            {
                var teamId = await UnitOfWork.Teams.Insert(team);
                if (teamId == 0)
                    return false;
            }
            else
            {
                team.Id = dbTeam.Id;
                var updated = await UnitOfWork.Teams.Update(team);
                if (updated == false)
                    return false;
            }
        }

        foreach (var t in dbTeams)
        {
            var item = teams.FirstOrDefault(x => x.Id == t.TeamId);
            if (item == null)
                await UnitOfWork.Teams.DeleteById(t.Id);
        }

        return true;
    }

    private async Task<bool> UpdateElements(Element[] elements, int seasonId)
    {
        var dbTeams = await UnitOfWork.Teams.GetAllBySeasonId(seasonId);

        var elementsDict = elements.GroupBy(x => x.Team).ToDictionary(x => x.Key, x => x.ToArray());
        foreach (var ed in elementsDict)
        {
            var team = dbTeams.FirstOrDefault(x => x.TeamId == ed.Key && x.SeasonId == seasonId);
            if (team == null)
                return false;

            var dbElements = await UnitOfWork.Elements.GetAllByTeamId(team.Id);
            foreach (var e in ed.Value)
            {
                var element = (Database.Models.Element)e;
                element.TeamId = dbTeams.FirstOrDefault(x => x.TeamId == e.Team)?.Id ?? 0;

                if (element.TeamId == 0)
                    return false;

                var dbElement = dbElements.FirstOrDefault(x => x.ElementTeamId == e.Team && x.ElementId == e.Id);
                if (dbElement == null)
                {
                    var elementId = await UnitOfWork.Elements.Insert(element);
                    if (elementId == 0)
                        return false;
                }
                else
                {
                    element.Id = dbElement.Id;
                    var updated = await UnitOfWork.Elements.Update(element);
                    if (updated == false)
                        return false;
                }
            }

            foreach (var e in dbElements)
            {
                var item = elements.FirstOrDefault(x => x.Id == e.ElementId);
                if (item == null)
                    await UnitOfWork.Elements.DeleteById(e.Id);
            }
        }

        return true;
    }

    private async Task<bool> UpdateElementStats(ElementStat[] stats, Database.Models.ElementStat[] dbStats, Database.Models.Element[] dbElements, int gameweek)
    {
        var gameweekStats = dbStats.Where(x => x.Gameweek == gameweek);
        foreach (var es in stats)
        {
            var s = (Database.Models.ElementStat)es.Stats;
            s.ApiElementId = es.Id;
            s.Gameweek = gameweek;
            s.ElementId = dbElements.FirstOrDefault(x => x.ElementId == es.Id).Id;

            var dbStat = gameweekStats.FirstOrDefault(x => x.ElementId == es.Id);
            if (dbStat == null)
            {
                var esId = await UnitOfWork.ElementStats.Insert(s);
                if (esId == 0)
                    return false;
            }
            else
            {
                s.ElementId = dbStat.Id;
                var updated = await UnitOfWork.ElementStats.Update(s);
                if (updated == false)
                    return false;
            }

        }

        return true;
    }

    private async Task<bool> UpdatePlayers(Player[] players, int leagueId)
    {
        var dbPlayers = await UnitOfWork.Players.GetAllByLeagueId(leagueId);

        foreach (var p in players)
        {
            var player = (Database.Models.Player)p;
            player.LeagueId = leagueId;

            var dbPlayer = dbPlayers.FirstOrDefault(x => x.EntryId == p.Entry);
            if (dbPlayer == null)
            {
                var playerId = await UnitOfWork.Players.Insert(player);
                if (playerId == 0)
                    return false;
            }
            else
            {
                player.Id = dbPlayer.Id;
                var updated = await UnitOfWork.Players.Update(player);
                if (updated == false)
                    return false;
            }
        }

        return true;
    }

    private async Task<bool> UpdatePicks(Pick[] picks, int gameweek, int leagueId, int entryId)
    {        
        var dbPlayer = await UnitOfWork.Players.Get(leagueId, entryId);
        var dbPicks = await UnitOfWork.Picks.GetAllByPlayerId(dbPlayer.Id);

        foreach (var p in picks)
        {
            var pick = (Database.Models.Pick)p;
            pick.PlayerId = dbPlayer.Id;
            pick.Gameweek = gameweek;

            var dbPick = dbPicks.FirstOrDefault(x => x.PlayerId == dbPlayer.Id && x.Gameweek == gameweek && x.Position == p.Position);
            if (dbPick == null)
            {
                var pickId = await UnitOfWork.Picks.Insert(pick);
                if (pickId == 0)
                    return false;
            }
            else
            {
                pick.Id = dbPick.Id;
                var updated = await UnitOfWork.Picks.Update(pick);
                if (updated == false)
                    return false;
            }
        }

        return true;
    }

    private async Task<bool> UpdatePoints(Points[] points, int leagueId, int entryId)
    {
        var dbPlayer = await UnitOfWork.Players.Get(leagueId, entryId);
        var dbPoints = await UnitOfWork.Points.GetAllByPlayerId(dbPlayer.Id);

        foreach (var p in points)
        {
            var point = (Database.Models.Points)p;
            point.PlayerId = dbPlayer.Id;

            var dbPoint = dbPoints.FirstOrDefault(x => x.PlayerId == dbPlayer.Id && x.Gameweek == p.Event);
            if (dbPoint == null)
            {
                var pointId = await UnitOfWork.Points.Insert(point);
                if (pointId == 0)
                    return false;
            }
            else
            {
                point.Id = dbPoint.Id;
                var updated = await UnitOfWork.Points.Update(point);
                if (updated == false)
                    return false;
            }
        }

        return true;
    }

}
