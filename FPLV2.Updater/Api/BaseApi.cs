using FPLV2.Database.Repositories.Interfaces;
using System.Diagnostics;

namespace FPLV2.Updater.Api;

/// <summary>
/// Base class that describes each different API call made to FPL
/// </summary>
public abstract class BaseApi
{
    /// <summary>
    /// Gets the order that the API call will be made. Order matters
    /// </summary>
    public abstract int Order { get; }
    /// <summary>
    /// The method that is called for each Api
    /// </summary>
    /// <returns>A task</returns>
    protected abstract Task Get();

    /// <summary>
    /// Gets or sets the FplClient which contains the methods to call the FPL API
    /// </summary>
    protected FplClient FplClient { get; init; }
    /// <summary>
    /// Gets or sets the UnitOfWork to call the Database
    /// </summary>
    protected IUnitOfWork UnitOfWork { get; init; }

    #region Globals
    /// <summary>
    /// Gets or sets the CurrentGameweek. This should be set in the BootstrapStatic API class and used across the other classes
    /// </summary>
    public static int CurrentGameweek { get; set; }

    /// <summary>
    /// Gets or sets the SeasonId. This should be set in the BootstrapStatic API class and used across the other classes
    /// </summary>
    public static int SeasonId { get; set; }
    #endregion

    /// <summary>
    /// Constructor of the class
    /// </summary>
    /// <param name="fplClient">Contains the methods to call the FPL API</param>
    /// <param name="unitOfWork">Contains the methods to call the Database</param>
    public BaseApi(FplClient fplClient, IUnitOfWork unitOfWork)
    {
        FplClient = fplClient;
        UnitOfWork = unitOfWork;
    }

    /// <summary>
    /// Call the method that is overridden by the implementation classes
    /// </summary>
    /// <returns>A task</returns>
    public async Task<bool> Execute()
    {
        try
        {
            await Get();
            return true;
        }
        catch (Exception ex)
        {
            await UnitOfWork.Logs.LogError(ex.ToString());
            return false;
        }
    }
}
