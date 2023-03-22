using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FPLV2.Database.Repositories;

public class LeagueRepository : BaseRepository, ILeagueRepository
{
    public LeagueRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<League[]> GetAll()
    {
        var sql = "select * from leagues";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<League>(sql);
        return result.ToArray();
    }

    public async Task<League[]> GetAllBySeasonId(int seasonId)
    {
        var sql = "select * from leagues where seasonid = @SeasonId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<League>(sql, new { SeasonId = seasonId });
        return result.ToArray();
    }

    public async Task DeleteAll()
    {
        var sql = "delete from players_in_leagues";
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(sql);

        sql = "delete from leagues";
        await conn.ExecuteAsync(sql);
    }

    public async Task<int> Insert(League league)
    {
        var sql = "insert into leagues (leagueId, seasonId, name, startevent) output inserted.id values (@LeagueId, @SeasonId, @Name, @StartEvent)";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteScalarAsync<int>(sql, league);
        return result;
    }

    public async Task<bool> Update(League league)
    {
        var sql = "update leagues set leagueid = @LeagueId, seasonid = @SeasonId, name = @Name, startevent = @StartEvent where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, league);
        var success = result > 0;
        return success;
    }
}
