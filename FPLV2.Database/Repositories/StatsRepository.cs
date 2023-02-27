using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FPLV2.Database.Repositories;

public class StatsRepository : BaseRepository, IStatsRepository
{
    public StatsRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<Stats[]> GetAll()
    {
        var sql = "select * from stats";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Stats>(sql);
        return result.ToArray();
    }

    public async Task<List<IDictionary<string, object>>> GetMostPointsInAGameweek(int seasonId, int leagueId)
    {
        var results = await GetStoredProcedure("1_mostpointsinagameweek", new { SeasonId = seasonId, LeagueId = leagueId });
        return results;
    }

    public async Task<List<IDictionary<string, object>>> GetMostBenchPointsInAGameweek(int seasonId, int leagueId)
    {
        var results = await GetStoredProcedure("1_mostbenchpointsinagameweek", new { SeasonId = seasonId, LeagueId = leagueId });
        return results;
    }

    private async Task<List<IDictionary<string, object>>> GetStoredProcedure(string procedureName, object parameters)
    {
        using var conn = await OpenConnection();
        var dbResults = await conn.QueryAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
        var results = dbResults.Select(x => (IDictionary<string, object>)x).ToList();
        return results;
    }
}
