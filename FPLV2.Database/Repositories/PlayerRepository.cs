using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FPLV2.Database.Repositories;

public class PlayerRepository : BaseRepository, IPlayerRepository
{
    public PlayerRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<Player> Get(int leagueId, int entryId)
    {
        var sql = "select * from players where leagueId = @LeagueId and entryId = @EntryId";
        using var conn = await OpenConnection();
        var result = await conn.QuerySingleOrDefaultAsync<Player>(sql, new { LeagueId = leagueId, EntryId = entryId });
        return result;
    }

    public async Task<Player[]> GetAll()
    {
        var sql = "select * from players";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Player>(sql);
        return result.ToArray();
    }

    public async Task<Player[]> GetAllByLeagueId(int leagueId)
    {
        var sql = "select * from players where leagueId = @LeagueId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Player>(sql, new { LeagueId = leagueId });
        return result.ToArray();
    }

    public async Task<bool> DeleteById(int id, SqlConnection conn = null)
    {
        var sql = "delete from players where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, new { Id = id });
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result > 0;
    }

    public async Task<int> Insert(Player player, SqlConnection conn = null)
    {
        var sql = "insert into players (leagueid, entryid, playername, teamname) output inserted.id values (@LeagueId, @EntryId, @PlayerName, @TeamName)";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteScalarAsync<int>(sql, player);
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result;
    }

    public async Task<bool> Update(Player player, SqlConnection conn = null)
    {
        var sql = "update players set leagueid = @LeagueId, entryid = @EntryId, playername = @PlayerName, teamname = @Teamname where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, player);
        var success = result > 0;
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return success;
    }

    public async Task<bool> ReplacePlayersByLeagueId(Player[] players, int leagueId)
    {
        var existItems = await GetAllByLeagueId(leagueId);
        var newIds = players.Select(x => x.EntryId).Except(existItems.Select(x => x.EntryId));
        var oldIds = existItems.Select(x => x.EntryId).Except(players.Select(x => x.EntryId));
        var sameIds = existItems.Select(x => x.EntryId).Intersect(players.Select(x => x.EntryId));

        using var conn = await OpenConnection();

        foreach (var id in newIds)
        {
            var player = players.FirstOrDefault(x => x.EntryId == id);
            if (player == null) continue;

            var result = await Insert(player, conn);
            if (result <= 0)
                return false;
        }

        foreach (var id in oldIds)
        {
            var player = existItems.FirstOrDefault(x => x.EntryId == id);
            if (player == null) continue;

            var result = await DeleteById(player.Id, conn);
            if (result == false)
                return false;
        }

        foreach (var id in sameIds)
        {
            var newPlayer = players.FirstOrDefault(x => x.EntryId == id);
            if (newPlayer == null) continue;
            var oldPlayer = existItems.FirstOrDefault(x => x.EntryId == id);
            if (oldPlayer == null) continue;

            newPlayer.Id = oldPlayer.Id;
            var hasChanged = JsonSerializer.Serialize(oldPlayer) != JsonSerializer.Serialize(newPlayer);
            if (hasChanged == false) continue;

            var result = await Update(newPlayer, conn);
            if (result == false)
                return false;
        }

        return true;
    }
}
