using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FPLV2.Database.Repositories;

/// <summary>
/// Repository that represents the logs table
/// </summary>
public class LoggingRepository : BaseRepository, ILoggingRepository
{
    /// <summary>
    /// Constructor for the LoggingRepository
    /// </summary>
    /// <param name="configuration">Configurations used for Repositories</param>
    public LoggingRepository(IConfiguration configuration) : base(configuration) { }

    /// <summary>
    /// Log a message of type Debug
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>A task</returns>
    public async Task LogDebug(string message)
    {
        var sql = "insert into logs (type, message, logtimeutc) values (@Type, @Message, @LogTimeUtc)";
        using var conn = await OpenConnection();
        await conn.ExecuteScalarAsync<int>(sql, new Logging() { Type = LogType.Debug, Message = message });
    }

    /// <summary>
    /// Log a message of type Information
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>A task</returns>
    public async Task LogInformation(string message)
    {
        var sql = "insert into logs (type, message, logtimeutc) values (@Type, @Message, @LogTimeUtc)";
        using var conn = await OpenConnection();
        await conn.ExecuteScalarAsync<int>(sql, new Logging() { Type = LogType.Information, Message = message });
    }

    /// <summary>
    /// Log a message of type Warning
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>A task</returns>
    public async Task LogWarning(string message)
    {
        var sql = "insert into logs (type, message, logtimeutc) values (@Type, @Message, @LogTimeUtc)";
        using var conn = await OpenConnection();
        await conn.ExecuteScalarAsync<int>(sql, new Logging() { Type = LogType.Warning, Message = message });
    }

    /// <summary>
    /// Log a message of type Error
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>A task</returns>
    public async Task LogError(string message)
    {
        var sql = "insert into logs (type, message, logtimeutc) values (@Type, @Message, @LogTimeUtc)";
        using var conn = await OpenConnection();
        await conn.ExecuteScalarAsync<int>(sql, new Logging() { Type = LogType.Error, Message = message });
    }
}
