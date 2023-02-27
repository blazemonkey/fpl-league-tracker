using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Updater.Models;

namespace FPLV2.Updater.Api;

/// <summary>
/// Calls the FPL API to get the ElementStats
/// </summary>
public class LiveApi : BaseApi
{
    /// <summary>
    /// Gets the order that the API call will be made. Order matters
    /// </summary>
    public override int Order => 5;

    /// <summary>
    /// Constructor of the class
    /// </summary>
    /// <param name="fplClient">Contains the methods to call the FPL API</param>
    /// <param name="unitOfWork">Contains the methods to call the Database</param>
    public LiveApi(FplClient fplClient, IUnitOfWork unitOfWork) : base(fplClient, unitOfWork) { }

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

        var latestGameweek = await UnitOfWork.ElementStats.GetLatestGameweekBySeasonId(SeasonId);
        for (var i = latestGameweek; i <= CurrentGameweek; i++)
        {
            var elementStats = await FplClient.GetElementStats(i);
            await UpdateElementStats(elementStats, SeasonId, i, dbElements);
        }
    }

    /// <summary>
    /// Takes the Element Stats that is returned from the API and replaces the ones in the Database
    /// </summary>
    /// <param name="elementStats">ElementStats returned from the API</param>
    /// <param name="seasonId">Id of the Season</param>
    /// <param name="gameweek">Gameweek for the stats</param>
    /// <param name="dbElements">Elements from Database used to set the ElementStats's ElementId</param>
    /// <returns>If the update was successful or not</returns>
    private async Task<bool> UpdateElementStats(ElementStat[] elementStats, int seasonId, int gameweek, Database.Models.Element[] dbElements)
    {
        var dbStats = new List<Database.Models.ElementStat>();
        foreach (var es in elementStats)
        {
            var dbEs = (Database.Models.ElementStat)es.Stats;
            dbEs.ApiElementId = es.Id;
            dbEs.Gameweek = gameweek;
            dbEs.ElementId = dbElements.FirstOrDefault(z => z.ElementId == es.Id)?.Id ?? 0;
            dbStats.Add(dbEs);
        }

        var result = await UnitOfWork.ElementStats.ReplaceElementStatsBySeasonId(dbStats.ToArray(), seasonId, gameweek);
        return result;
    }
}
