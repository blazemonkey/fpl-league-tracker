using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FPLV2.Database.Repositories;

public class PickRepository : BaseRepository, IPickRepository
{
    public PickRepository(IConfiguration configuration) : base(configuration) { }
    public async Task<Pick[]> GetAllByPlayerId(int playerId)
    {
        var sql = "select * from picks where playerId = @PlayerId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Pick>(sql, new { PlayerId = playerId });
        return result.ToArray();
    }

    public async Task<Pick[]> GetAllByPlayerId(int playerId, int gameweek)
    {
        var sql = "select * from picks where playerId = @PlayerId and gameweek = @Gameweek";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Pick>(sql, new { PlayerId = playerId, Gameweek = gameweek });
        return result.ToArray();
    }

    public async Task<int> GetLatestGameweekByPlayerId(int playerId)
    {
        var sql = "select max(gameweek) from picks where playerId = @PlayerId";
        using var conn = await OpenConnection();
        var result = await conn.QuerySingleOrDefaultAsync<int?>(sql, new { PlayerId = playerId }) ?? 1; // gameweek 1 is by default the first one we want
        return result;
    }
    public async Task<bool> DeleteById(int id, SqlConnection conn = null)
    {
        var sql = "delete from picks where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, new { Id = id });
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result > 0;
    }

    public async Task<int> Insert(Pick pick, SqlConnection conn = null)
    {
        var sql = "insert into picks (playerId, gameweek, elementId, multiplier, position) output inserted.id values (@PlayerId, @Gameweek, @ElementId, @Multiplier, @Position)";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteScalarAsync<int>(sql, pick);
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result;
    }

    public async Task<bool> Update(Pick pick, SqlConnection conn = null)
    {
        var sql = "update picks set playerId = @PlayerId, gameweek = @Gameweek, elementId = @ElementId, multiplier = @Multiplier, position = @Position where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, pick);
        var success = result > 0;
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return success;
    }

    public async Task<bool> ReplacePicksByPlayerId(Pick[] picks, int playerId, int gameweek)
    {
        var existItems = await GetAllByPlayerId(playerId, gameweek);
        var newItems = picks.Select(x => new { x.PlayerId, x.Gameweek, x.Position }).Except(existItems.Select(x => new { x.PlayerId, x.Gameweek, x.Position }));
        var oldItems = existItems.Select(x => new { x.PlayerId, x.Gameweek, x.Position }).Except(picks.Select(x => new { x.PlayerId, x.Gameweek, x.Position }));
        var sameItems = existItems.Select(x => new { x.PlayerId, x.Gameweek, x.Position }).Intersect(picks.Select(x => new { x.PlayerId, x.Gameweek, x.Position }));

        using var conn = await OpenConnection();

        foreach (var i in newItems)
        {
            var pick = picks.FirstOrDefault(x => x.PlayerId == i.PlayerId && x.Gameweek == i.Gameweek && x.Position == i.Position);
            if (pick == null) continue;

            var result = await Insert(pick, conn);
            if (result <= 0)
                return false;
        }

        foreach (var i in oldItems)
        {
            var pick = existItems.FirstOrDefault(x => x.PlayerId == i.PlayerId && x.Gameweek == i.Gameweek && x.Position == i.Position);
            if (pick == null) continue;

            var result = await DeleteById(pick.Id, conn);
            if (result == false)
                return false;
        }

        foreach (var i in sameItems)
        {
            var newPick = picks.FirstOrDefault(x => x.PlayerId == i.PlayerId && x.Gameweek == i.Gameweek && x.Position == i.Position);
            if (newPick == null) continue;
            var oldPick = existItems.FirstOrDefault(x => x.PlayerId == i.PlayerId && x.Gameweek == i.Gameweek && x.Position == i.Position);
            if (oldPick == null) continue;

            newPick.Id = oldPick.Id;
            var hasChanged = JsonSerializer.Serialize(oldPick) != JsonSerializer.Serialize(newPick);
            if (hasChanged == false) continue;

            var result = await Update(newPick, conn);
            if (result == false)
                return false;
        }

        return true;
    }
}
