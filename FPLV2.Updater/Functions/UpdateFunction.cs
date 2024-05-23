using FPLV2.Client;
using FPLV2.Client.Models;
using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Updater.Api;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FPLV2.Updater.Functions;

/// <summary>
/// Periodically update data about an FPL season, including any Mini Leagues that are in the Leagues table
/// </summary>
public class UpdateFunction : Function
{
    public FplClient FplClient { get; init; }

    public UpdateFunction(ILoggerFactory loggerFactory, IConfiguration configuration, FplClient fplClient, IUnitOfWork unitOfWork) : base(loggerFactory, configuration, fplClient, unitOfWork)
    {
        Logger = loggerFactory.CreateLogger<UpdateFunction>();
        FplClient = fplClient;
    }

    [Function("UpdateFunction")]
#if DEBUG
    public async Task Run([TimerTrigger("0 * * * * *")] FunctionInfo info)
#else
    public async Task Run([TimerTrigger("0 0 */1 * * *")] FunctionInfo info)
#endif
    {
        try
        {
            Logger.LogInformation($"{nameof(UpdateFunction)} executed at: {DateTime.Now}");

            var calls = GetApiCalls();

            foreach (var c in calls)
            {
                var success = await c.Execute();
                if (success == false) // stop executing if an error is found in any of the API calls
                    break;
            }

        }
        catch (Exception ex)
        {
            Logger.LogError($"{nameof(UpdateFunction)} error occured: {ex}");
        }
        finally
        {
            Logger.LogInformation($"{nameof(UpdateFunction)} completed at: {DateTime.Now}");
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

    /// <summary>
    /// Inserts the Season into the database if it doesn't exist yet
    /// </summary>
    /// <param name="result">The Id of the Season</param>
    /// <returns>Season Id</returns>
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
}
