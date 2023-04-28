using FPLV2.Client;
using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Updater.Api;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
#if DEBUG
    public async Task Run([TimerTrigger("0 * * * * *")] FunctionInfo info)
#else
    public async Task Run([TimerTrigger("0 0 */1 * * *")] FunctionInfo info)
#endif
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
}
