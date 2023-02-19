using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Updater.Models;

namespace FPLV2.Updater.Api;

/// <summary>
/// Calls the FPL API to get the Picks that a Player makes
/// </summary>
public class PicksApi : BaseApi
{
    /// <summary>
    /// Gets the order that the API call will be made. Order matters
    /// </summary>
    public override int Order => 3;

    /// <summary>
    /// Constructor of the class
    /// </summary>
    /// <param name="fplClient">Contains the methods to call the FPL API</param>
    /// <param name="unitOfWork">Contains the methods to call the Database</param>
    public PicksApi(FplClient fplClient, IUnitOfWork unitOfWork) : base(fplClient, unitOfWork) { }

    /// <summary>
    /// Call the FPL API to retrieve the information
    /// </summary>
    /// <returns>A task</returns>
    protected override async Task Get()
    {
        if (SeasonId == 0)
            throw new Exception("No SeasonId has been set");

        if (CurrentGameweek == 0)
            throw new Exception("No CurrentGameweek has been set");

        var dbElements = await UnitOfWork.Elements.GetAllBySeasonId(SeasonId);

        var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(SeasonId);
        foreach (var l in leagues)
        {
            // get players
            var players = await UnitOfWork.Players.GetAllByLeagueId(l.Id);
            if (players.Any() == false)
                continue;

            foreach (var p in players)
            {
                var latestGameweek = await UnitOfWork.Picks.GetLatestGameweekByPlayerId(p.Id);
                if (latestGameweek > CurrentGameweek) // this wouldn't make sense
                    continue;

                for (var i = latestGameweek; i <= CurrentGameweek; i++)
                {
                    var picks = await FplClient.GetPicks(p.EntryId, i);
                    await UpdatePicks(picks, p.Id, i, dbElements);
                }
            }
        }
    }

    /// <summary>
    /// Takes the Picks that is returned from the API and replaces the ones in the Database
    /// </summary>
    /// <param name="picks">Picks returned from the API</param>
    /// <param name="playerId">Id of the Player</param>
    /// <param name="gameweek">Gameweek for the picks</param>
    /// <param name="dbElements">Elements from Database used to set the Pick's ElementId</param>
    /// <returns>If the update was successful or not</returns>
    private async Task<bool> UpdatePicks(Pick[] picks, int playerId, int gameweek, Database.Models.Element[] dbElements)
    {
        var dbPicks = picks?.Select(x => (Database.Models.Pick)x)?.ToList() ?? new List<Database.Models.Pick>();
        dbPicks.ForEach(x =>
        {
            x.PlayerId = playerId;
            x.Gameweek = gameweek;
            x.ElementId = dbElements.FirstOrDefault(z => z.ElementId == x.ElementId)?.Id ?? 0;
        });
        var result = await UnitOfWork.Picks.ReplacePicksByPlayerId(dbPicks.ToArray(), playerId, gameweek);
        return result;
    }
}
