using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Updater.Api;
using FPLV2.Updater.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
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
        try
        {
            Logger.LogInformation($"UpdateFunction executed at: {DateTime.Now}");

            var calls = GetApiCalls();

            foreach (var c in calls)
            {
                var success = await c.Execute();
                if (success == false) // stop executing if an error is found in any of the API calls
                    break;
            }

            //var result = await FplClient.GetBootstrapStatic();
            //if (result == null)
            //    return;

            //var currentGameweek = result.Gameweeks.FirstOrDefault(x => x.IsCurrent);
            //if (currentGameweek == null)
            //    return;

            //// update seasons
            //var seasonId = await UpdateSeasons(result);
            //if (seasonId == 0)
            //    return;

            //// update teams
            //var updatedTeams = await UpdateTeams(result.Teams ?? new Team[] { }, seasonId);
            //if (updatedTeams == false)
            //    return;

            //// update elements
            //var updatedElements = await UpdateElements(result.Elements ?? new Element[] { }, seasonId);
            //if (updatedElements == false)
            //    return;

            //var dbElementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
            //var dbElements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
            //for (var i = 1; i <= currentGameweek.Id; i++)
            //{
            //    // update element stats
            //    var elementStats = await FplClient.GetElementStats(i);
            //    await UpdateElementStats(elementStats, dbElementStats, dbElements, i);
            //}

            //// loop leagues that need to be updated
            //var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(seasonId);
            //foreach (var l in leagues)
            //{
            //    // get league standings
            //    var standings = await FplClient.GetLeagueStandings(l.LeagueId);
            //    if (standings == null)
            //        continue;

            //    // league type must be 'x', which are the custom created leagues
            //    if (standings.League.LeagueType != "x")
            //        continue;

            //    // update league
            //    l.Name = standings.League.Name;
            //    var updatedLeague = await UnitOfWork.Leagues.Update(l);
            //    if (updatedLeague == false)
            //        continue;

            //    // update players in league
            //    await UpdatePlayers(standings.Results.Players, l.LeagueId);

            //    foreach (var p in standings.Results.Players)
            //    {
            //        for (var i = 1; i <= currentGameweek.Id; i++)
            //        {
            //            // update picks
            //            var picks = await FplClient.GetPicks(p.Entry, i);
            //            await UpdatePicks(picks, i, l.LeagueId, p.Entry);
            //        }

            //        // update points
            //        var points = await FplClient.GetPointsHistory(p.Entry);
            //        await UpdatePoints(points, l.LeagueId, p.Entry);
            //    }
            //}

        }
        catch (Exception ex)
        {
            Logger.LogInformation($"UpdateFunction error occured: {ex}");
        }
        finally
        {
            Logger.LogInformation($"UpdateFunction completed at: {DateTime.Now}");
        }
    }

    /// <summary>
    /// Get the API classes that implement the BaseApi class. These classes are the ones that make the call to the FPL API
    /// </summary>
    /// <returns>An array of classes that implement the BaseApi class</returns>
    private BaseApi[] GetApiCalls()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(BaseApi)) && x.IsAbstract == false).ToArray();
        var calls = new List<BaseApi>();

        foreach (var c in types)
        {
            var instance = (BaseApi)Activator.CreateInstance(c, FplClient, UnitOfWork);
            calls.Add(instance);
        }

        return calls.OrderBy(x => x.Order).ToArray();
    }

    private async Task<bool> UpdateElementStats(ElementStat[] stats, Database.Models.ElementStat[] dbStats, Database.Models.Element[] dbElements, int gameweek)
    {
        var gameweekStats = dbStats.Where(x => x.Gameweek == gameweek);
        foreach (var es in stats ?? new ElementStat[] { })
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
}
