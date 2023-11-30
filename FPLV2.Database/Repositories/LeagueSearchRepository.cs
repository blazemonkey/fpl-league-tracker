using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FPLV2.Database.Repositories;

public class LeagueSearchRepository : BaseRepository, ILeagueSearchRepository
{
    public LeagueSearchRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<LeagueSearch[]> GetAll()
    {
        var sql = "select * from leagues_search";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<LeagueSearch>(sql);
        return result.ToArray();
    }

    public async Task<LeagueSearch[]> GetAllBySeasonId(int seasonId)
    {
        var sql = "select * from leagues_search where seasonid = @SeasonId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<LeagueSearch>(sql, new { SeasonId = seasonId });
        return result.ToArray();
    }

    public async Task<int> GetMaxLeagueId(int seasonId)
    {
        var sql = "select max(LeagueId) from leagues_search where seasonid = @SeasonId";
        using var conn = await OpenConnection();
        var result = await conn.QuerySingleOrDefaultAsync<int?>(sql, new { SeasonId = seasonId });
        return result ?? 0;
    }

    public async Task DeleteAll()
    {
        var sql = "delete from leagues_search";
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(sql);
    }

    public async Task<int> Insert(LeagueSearch league)
    {
        var sql = "insert into leagues_search (leagueId, seasonId, name, leagueType, createdTimeUtc, adminEntryId, adminPlayerName) output inserted.id values (@LeagueId, @SeasonId, @Name, @LeagueType, @AdminEntryId, @AdminPlayerName, @CreatedTimeUtc)";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteScalarAsync<int>(sql, league);
        return result;
    }

    public async Task<int> InsertAll(LeagueSearch[] leagues)
    {
        var sql = "insert into leagues_search (leagueId, seasonId, name, leagueType, createdTimeUtc, adminEntryId, adminPlayerName) output inserted.id values (@LeagueId, @SeasonId, @Name, @LeagueType, @CreatedTimeUtc, @AdminEntryId, @AdminPlayerName)";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, leagues);
        return result;
    }

    public async Task<bool> Update(LeagueSearch league)
    {
        var sql = "update leagues set leagueid = @LeagueId, seasonid = @SeasonId, name = @Name, leagueType = @LeagueType, createdTimeUtc = @CreatedTimeUtc, adminEntryId = @AdminEntryId, adminPlayerName = @AdminPlayerName where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, league);
        var success = result > 0;
        return success;
    }
}
