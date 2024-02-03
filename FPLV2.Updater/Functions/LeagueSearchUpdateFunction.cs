using FPLV2.Client;
using FPLV2.Client.Models;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Updater.Api;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FPLV2.Updater.Functions;

/// <summary>
/// Update the League Search table, to allow searching Leagues by name
/// </summary>
public class LeagueSearchUpdateFunction : Function
{
    public FplClient FplClient { get; init; }

    public LeagueSearchUpdateFunction(ILoggerFactory loggerFactory, IConfiguration configuration, FplClient fplClient, IUnitOfWork unitOfWork) : base(loggerFactory, configuration, fplClient, unitOfWork)
    {
        Logger = loggerFactory.CreateLogger<LeagueSearchUpdateFunction>();
        FplClient = fplClient;
    }

    [Function("LeagueSearchUpdateFunction")]
#if DEBUG
    public async Task Run([TimerTrigger("0 */2 * * * *")] FunctionInfo info)
#else
    public async Task Run([TimerTrigger("0 */2 * * * *")] FunctionInfo info)
#endif
    {
        try
        {
            Logger.LogInformation($"LeagueSearchUpdateFunction executed at: {DateTime.Now}");

            var result = await FplClient.GetBootstrapStatic();
            if (result == null)
                throw new Exception("Could not get data");

            // update seasons
            var seasonId = await GetSeasonId(result);
            if (seasonId == 0)
                throw new Exception("Could not get the SeasonId");

            var leagues = new List<LeagueSearch>();
            var maxLeagueId = await UnitOfWork.LeagueSearch.GetMaxLeagueId(seasonId) + 1;

            // for every minute, make 100 calls
            for (var i = maxLeagueId; i < maxLeagueId + 100; i++)
            {
                var league = await GetLeague(i, seasonId);
                if (league == null)
                    continue;

                leagues.Add(league);
            }

            await UnitOfWork.LeagueSearch.InsertAll(leagues.ToArray());

        }
        catch (Exception ex)
        {
            Logger.LogError($"LeagueSearchUpdateFunction error occured: {ex}");
        }
        finally
        {
            Logger.LogInformation($"LeagueSearchUpdateFunction completed at: {DateTime.Now}");
        }
    }

    private async Task<LeagueSearch> GetLeague(int leagueId, int seasonId)
    {
        try
        {
            // get league standings
            var standings = await FplClient.GetLeagueStandings(leagueId, false);
            int? adminEntryId = null;
            string adminPlayerName = null;
            if (standings.League.AdminEntry != null)
            {
                var entry = await FplClient.GetEntry(standings.League.AdminEntry.Value);
                adminEntryId = entry.Id;
                adminPlayerName = entry.PlayerName;
            }

            var leagueSearch = new LeagueSearch()
            {
                SeasonId = seasonId,
                LeagueId = leagueId,
                Name = standings.League.Name,
                LeagueType = standings.League.LeagueType,
                CreatedTimeUtc = standings.League.Created,
                AdminEntryId = adminEntryId,
                AdminPlayerName = adminPlayerName
            };

            return leagueSearch;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Get the SeasonId from the database. It is expected that this exists already (added from the UpdateFunction), if it doesn't it will not attempt to add it
    /// </summary>
    /// <param name="result">The Id of the Season</param>
    /// <returns>Season Id</returns>
    private async Task<int> GetSeasonId(BootstrapStatic result)
    {
        var openingDate = result.Gameweeks?.FirstOrDefault()?.DeadlineTime ?? DateTime.MinValue;
        var finalDate = result.Gameweeks?.LastOrDefault()?.DeadlineTime ?? DateTime.MinValue;
        if (openingDate == DateTime.MinValue || finalDate == DateTime.MinValue)
            return 0;

        var seasonYear = $"{openingDate.Year}/{finalDate.Year.ToString().Substring(2, 2)}"; // e.g. 2020/21
        var dbSeasons = await UnitOfWork.Seasons.GetAll();

        var seasonId = dbSeasons.FirstOrDefault(x => x.Year == seasonYear)?.Id ?? 0;      
        return seasonId;
    }
}
