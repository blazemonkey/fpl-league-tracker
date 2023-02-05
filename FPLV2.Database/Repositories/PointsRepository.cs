using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FPLV2.Database.Repositories;

public class PointsRepository : BaseRepository, IPointsRepository
{
    public PointsRepository(IConfiguration configuration) : base(configuration) { }
    public async Task<Points[]> GetAllByPlayerId(int playerId)
    {
        var sql = "select * from points where playerId = @PlayerId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Points>(sql, new { PlayerId = playerId });
        return result.ToArray();
    }

    public async Task<int> Insert(Points points)
    {
        var sql = "insert into points (playerId, gameweek, gameweekpoints, gameweekpointsonbench, total) output inserted.id values (@PlayerId, @Gameweek, @GameweekPoints, @GameweekPointsOnBench, @Total)";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteScalarAsync<int>(sql, points);
        return result;
    }

    public async Task<bool> Update(Points points)
    {
        var sql = "update points set playerId = @PlayerId, gameweek = @Gameweek, gameweekpoints = @GameweekPoints, gameweekpointsonbench = @GameweekPointsOnBench, total = @Total where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, points);
        var success = result > 0;
        return success;
    }
}
