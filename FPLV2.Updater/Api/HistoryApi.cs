using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Updater.Models;

namespace FPLV2.Updater.Api;

/// <summary>
/// Calls the FPL API to get the Points History for a Entry
/// </summary>
public class HistoryApi : BaseApi
{
    /// <summary>
    /// Gets the order that the API call will be made. Order matters
    /// </summary>
    public override int Order => 4;

    /// <summary>
    /// Constructor of the class
    /// </summary>
    /// <param name="fplClient">Contains the methods to call the FPL API</param>
    /// <param name="unitOfWork">Contains the methods to call the Database</param>
    public HistoryApi(FplClient fplClient, IUnitOfWork unitOfWork) : base(fplClient, unitOfWork) { }

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
            // get players
            var players = await UnitOfWork.Players.GetAllByLeagueId(l.Id);
            if (players.Any() == false)
                continue;

            foreach (var p in players)
            {
                var points = await FplClient.GetPointsHistory(p.EntryId);
                await UpdatePoints(points, p.Id);
            }
        }
    }

    /// <summary>
    /// Takes the Points that is returned from the API and replaces the ones in the Database
    /// </summary>
    /// <param name="points">Points returned from the API</param>
    /// <param name="playerId">Id of the Player</param>
    /// <returns>If the update was successful or not</returns>
    private async Task<bool> UpdatePoints(Points[] points, int playerId)
    {
        var dbPoints = points?.Select(x => (Database.Models.Points)x)?.ToList() ?? new List<Database.Models.Points>();
        dbPoints.ForEach(x => x.PlayerId = playerId);
        var result = await UnitOfWork.Points.ReplacePointsByPlayerId(dbPoints.ToArray(), playerId);
        return result;
    }
}
