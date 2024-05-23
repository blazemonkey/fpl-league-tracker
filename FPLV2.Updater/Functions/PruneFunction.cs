using FPLV2.Client;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FPLV2.Updater.Functions;

/// <summary>
/// Periodically cleans tables in the database
/// </summary>
public class PruneFunction : Function
{
    public FplClient FplClient { get; init; }

    public PruneFunction(ILoggerFactory loggerFactory, IConfiguration configuration, FplClient fplClient, IUnitOfWork unitOfWork) : base(loggerFactory, configuration, fplClient, unitOfWork)
    {
        Logger = loggerFactory.CreateLogger<PruneFunction>();
        FplClient = fplClient;
    }

    [Function("PruneFunction")]
    public async Task Run([TimerTrigger("0 0 0 * * *")] FunctionInfo info)
    {
        try
        {
            Logger.LogInformation($"{nameof(PruneFunction)} executed at: {DateTime.Now}");
            await UnitOfWork.Logs.Prune();

        }
        catch (Exception ex)
        {
            Logger.LogError($"{nameof(PruneFunction)} error occured: {ex}");
        }
        finally
        {
            Logger.LogInformation($"{nameof(PruneFunction)} completed at: {DateTime.Now}");
        }
    }
}
