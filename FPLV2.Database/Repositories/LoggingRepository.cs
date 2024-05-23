using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FPLV2.Database.Repositories;

/// <inheritdoc/>
public class LoggingRepository : BaseRepository, ILoggingRepository
{
    /// <summary>
    /// Constructor for the LoggingRepository
    /// </summary>
    /// <param name="configuration">Configurations used for Repositories</param>
    public LoggingRepository(IConfiguration configuration) : base(configuration) { }

    /// <inheritdoc/>
    public async Task LogDebug(string message)
    {
        var sql = "insert into logs (type, message, logtimeutc) values (@Type, @Message, @LogTimeUtc)";
        using var conn = await OpenConnection();
        await conn.ExecuteScalarAsync<int>(sql, new Logging() { Type = LogType.Debug, Message = message });
    }

    /// <inheritdoc/>
    public async Task LogInformation(string message)
    {
        var sql = "insert into logs (type, message, logtimeutc) values (@Type, @Message, @LogTimeUtc)";
        using var conn = await OpenConnection();
        await conn.ExecuteScalarAsync<int>(sql, new Logging() { Type = LogType.Information, Message = message });
    }

    /// <inheritdoc/>
    public async Task LogWarning(string message)
    {
        var sql = "insert into logs (type, message, logtimeutc) values (@Type, @Message, @LogTimeUtc)";
        using var conn = await OpenConnection();
        await conn.ExecuteScalarAsync<int>(sql, new Logging() { Type = LogType.Warning, Message = message });
    }

    /// <inheritdoc/>
    public async Task LogError(string message)
    {
        var sql = "insert into logs (type, message, logtimeutc) values (@Type, @Message, @LogTimeUtc)";
        using var conn = await OpenConnection();
        await conn.ExecuteScalarAsync<int>(sql, new Logging() { Type = LogType.Error, Message = message });
    }

    /// <inheritdoc/>
    public async Task Prune()
    {
        var sql = "delete from logs where logtimeutc < DATEADD(MONTH, -1, GETDATE())";
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(sql);
    }
}
