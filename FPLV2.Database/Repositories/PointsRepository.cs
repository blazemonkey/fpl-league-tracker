using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

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

    public async Task<Points[]> GetAllByPlayerId(int playerId, int gameweek)
    {
        var sql = "select * from points where playerId = @PlayerId and gameweek = @Gameweek";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Points>(sql, new { PlayerId = playerId, Gameweek = gameweek });
        return result.ToArray();
    }

    public async Task<int> Insert(Points points, SqlConnection conn = null)
    {
        var sql = "insert into points (playerId, gameweek, gameweekpoints, gameweekpointsonbench, total) output inserted.id values (@PlayerId, @Gameweek, @GameweekPoints, @GameweekPointsOnBench, @Total)";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteScalarAsync<int>(sql, points);
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result;
    }

    public async Task<bool> Update(Points points, SqlConnection conn = null)
    {
        var sql = "update points set playerId = @PlayerId, gameweek = @Gameweek, gameweekpoints = @GameweekPoints, gameweekpointsonbench = @GameweekPointsOnBench, total = @Total where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, points);
        var success = result > 0;
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return success;
    }
    public async Task<bool> DeleteById(int id, SqlConnection conn = null)
    {
        var sql = "delete from points where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, new { Id = id });
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result > 0;
    }

    public async Task<bool> ReplacePointsByPlayerId(Points[] points, int playerId)
    {
        var existItems = await GetAllByPlayerId(playerId);
        var newItems = points.Select(x => new { x.PlayerId, x.Gameweek }).Except(existItems.Select(x => new { x.PlayerId, x.Gameweek }));
        var oldItems = existItems.Select(x => new { x.PlayerId, x.Gameweek }).Except(points.Select(x => new { x.PlayerId, x.Gameweek }));
        var sameItems = existItems.Select(x => new { x.PlayerId, x.Gameweek }).Intersect(points.Select(x => new { x.PlayerId, x.Gameweek }));

        using var conn = await OpenConnection();

        foreach (var i in newItems)
        {
            var point = points.FirstOrDefault(x => x.PlayerId == i.PlayerId && x.Gameweek == i.Gameweek);
            if (point == null) continue;

            var result = await Insert(point, conn);
            if (result <= 0)
                return false;
        }

        foreach (var i in oldItems)
        {
            var point = existItems.FirstOrDefault(x => x.PlayerId == i.PlayerId && x.Gameweek == i.Gameweek);
            if (point == null) continue;

            var result = await DeleteById(point.Id, conn);
            if (result == false)
                return false;
        }

        foreach (var i in sameItems)
        {
            var newPoint = points.FirstOrDefault(x => x.PlayerId == i.PlayerId && x.Gameweek == i.Gameweek);
            if (newPoint == null) continue;
            var oldPoint = existItems.FirstOrDefault(x => x.PlayerId == i.PlayerId && x.Gameweek == i.Gameweek);
            if (oldPoint == null) continue;

            newPoint.Id = oldPoint.Id;
            var hasChanged = JsonSerializer.Serialize(oldPoint) != JsonSerializer.Serialize(newPoint);
            if (hasChanged == false) continue;

            var result = await Update(newPoint, conn);
            if (result == false)
                return false;
        }

        return true;
    }
}
