using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Client.Models;
using FPLV2.Client;

namespace FPLV2.Updater.Api;

/// <summary>
/// Calls the FPL API to get the initial data
/// </summary>
public class BootstrapStaticApi : BaseApi
{
    /// <summary>
    /// Gets the order that the API call will be made. Order matters
    /// </summary>
    public override int Order => 1;

    /// <summary>
    /// Constructor of the class
    /// </summary>
    /// <param name="fplClient">Contains the methods to call the FPL API</param>
    /// <param name="unitOfWork">Contains the methods to call the Database</param>
    public BootstrapStaticApi(FplClient fplClient, IUnitOfWork unitOfWork) : base(fplClient, unitOfWork) { }

    /// <summary>
    /// Call the FPL API to retrieve the information
    /// </summary>
    /// <returns>A task</returns>
    protected override async Task Get()
    {
        var result = await FplClient.GetBootstrapStatic();
        if (result == null)
            throw new Exception("Could not get data");

        // get the current gameweek
        var currentGameweek = result.Gameweeks.FirstOrDefault(x => x.IsCurrent);
        if (currentGameweek == null)
            throw new Exception("There is no current gameweek");

        // update seasons
        var seasonId = await UpdateSeasons(result);
        if (seasonId == 0)
            throw new Exception("Could not get the SeasonId");

        // update teams
        var updatedTeams = await UpdateTeams(result.Teams ?? new Team[] { }, seasonId);
        if (updatedTeams == false)
            throw new Exception("An error occured while updating Teams");

        // update elements
        var updatedElements = await UpdateElements(result.Elements ?? new Element[] { }, seasonId);
        if (updatedElements == false)
            throw new Exception("An error occured while updating Elements");

        CurrentGameweek = currentGameweek.Id;
        SeasonId = seasonId;
    }

    /// <summary>
    /// Inserts the Season into the database if it doesn't exist yet
    /// </summary>
    /// <param name="result">The Id of the Season</param>
    /// <returns></returns>
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

    /// <summary>
    /// Takes the Teams that is returned from the API and replaces the ones in the Database
    /// </summary>
    /// <param name="teams">Teams returned from the API</param>
    /// <param name="seasonId">Current SeasonId</param>
    /// <returns>If the update was successful or not</returns>
    private async Task<bool> UpdateTeams(Team[] teams, int seasonId)
    {
        var dbTeams = teams?.Select(x => (Database.Models.Team)x)?.ToList() ?? new List<Database.Models.Team>();
        dbTeams.ForEach(x => x.SeasonId = seasonId);
        var result = await UnitOfWork.Teams.ReplaceTeamsBySeasonId(dbTeams.ToArray(), seasonId);
        return result;
    }

    /// <summary>
    /// Takes the Elements that is returned from the API and replaces the ones in the Database
    /// </summary>
    /// <param name="elements">Elements returned from the API</param>
    /// <param name="seasonId">Current SeasonId</param>
    /// <returns>If the update was successful or not</returns>
    private async Task<bool> UpdateElements(Element[] elements, int seasonId)
    {
        var teams = await UnitOfWork.Teams.GetAllBySeasonId(seasonId);

        var dbElements = elements?.Select(x => (Database.Models.Element)x)?.ToList() ?? new List<Database.Models.Element>();
        dbElements.ForEach(x => x.TeamId = teams.First(z => z.TeamId == x.ElementTeamId).Id);
        var result = await UnitOfWork.Elements.ReplaceElementsBySeasonId(dbElements.ToArray(), seasonId);
        return result;
    }
}
