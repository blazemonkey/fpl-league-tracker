namespace FPLV2.Database.Repositories.Interfaces;

/// <summary>
/// Repository that represents the logs table
/// </summary>
public interface ILoggingRepository
{
    /// <summary>
    /// Log a message of type Debug
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>A task</returns>
    Task LogDebug(string message);
    /// <summary>
    /// Log a message of type Information
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>A task</returns>
    Task LogInformation(string message);
    /// <summary>
    /// Log a message of type Warning
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>A task</returns>
    Task LogWarning(string message);
    /// <summary>
    /// Log a message of type Error
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>A task</returns>
    Task LogError(string message);

    /// <summary>
    /// Prunes the table of records older than 1 month ago
    /// </summary>
    /// <returns>A task</returns>
    Task Prune();
}
