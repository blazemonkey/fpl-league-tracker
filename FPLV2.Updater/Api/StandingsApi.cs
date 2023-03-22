using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Updater.Models;

namespace FPLV2.Updater.Api;

/// <summary>
/// Calls the FPL API to get the League Standings and Players
/// </summary>
public class StandingsApi : BaseApi
{
    /// <summary>
    /// Gets the order that the API call will be made. Order matters
    /// </summary>
    public override int Order => 2;

    /// <summary>
    /// Constructor of the class
    /// </summary>
    /// <param name="fplClient">Contains the methods to call the FPL API</param>
    /// <param name="unitOfWork">Contains the methods to call the Database</param>
    public StandingsApi(FplClient fplClient, IUnitOfWork unitOfWork) : base(fplClient, unitOfWork) { }

    /// <summary>
    /// Call the FPL API to retrieve the information
    /// </summary>
    /// <returns>A task</returns>
    protected override async Task Get()
    {
        if (SeasonId == 0)
            throw new Exception("No SeasonId has been set");

        var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(SeasonId);
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
            l.StartEvent = standings.League.StartEvent;
            var updatedLeague = await UnitOfWork.Leagues.Update(l);
            if (updatedLeague == false)
                continue;

            await UpdatePlayers(standings.Results.Players, l.Id);
        }
    }

    /// <summary>
    /// Takes the Players that is returned from the API and replaces the ones in the Database
    /// </summary>
    /// <param name="players">Players returned from the API</param>
    /// <param name="leagueId">Id of the League</param>
    /// <returns>If the update was successful or not</returns>
    private async Task<bool> UpdatePlayers(Player[] players, int leagueId)
    {
        var dbPlayers = players?.Select(x => (Database.Models.Player)x)?.ToList() ?? new List<Database.Models.Player>();
        dbPlayers.ForEach(x => x.LeagueId = leagueId);
        var result = await UnitOfWork.Players.ReplacePlayersByLeagueId(dbPlayers.ToArray(), leagueId, SeasonId);
        return result;
    }
}
