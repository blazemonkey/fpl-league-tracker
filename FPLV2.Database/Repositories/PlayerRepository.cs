using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Numerics;
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
        var sql = "select p.* from players p join players_in_leagues pl on p.id = pl.playerid where pl.leagueId = @LeagueId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Player>(sql, new { LeagueId = leagueId });
        return result.ToArray();
    }

    public async Task<Player[]> GetAllBySeasonId(int seasonId)
    {
        var sql = "select distinct p.* from players p join players_in_leagues pl on p.id = pl.playerid join leagues l on l.id = pl.leagueid where l.seasonid = @SeasonId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Player>(sql, new { SeasonId = seasonId });
        return result.ToArray();
    }

    public async Task DeleteAll()
    {
        var sql = "delete from players_in_leagues";
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(sql);

        sql = "delete from players";
        await conn.ExecuteAsync(sql);
    }

    public async Task<bool> DeleteById(int id, SqlConnection conn = null)
    {
        await new PlayerInLeagueRepository(Configuration).DeleteByPlayerId(id);

        var sql = "delete from players where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, new { Id = id });

        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result > 0;
    }

    public async Task<int> Insert(Player player, SqlConnection conn = null)
    {
        var sql = "insert into players (entryid, playername, teamname) output inserted.id values (@EntryId, @PlayerName, @TeamName)";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteScalarAsync<int>(sql, player);

        var playerInLeague = new PlayerInLeague() { PlayerId = result, LeagueId = player.LeagueId };
        await new PlayerInLeagueRepository(Configuration).Insert(playerInLeague, conn);

        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result;
    }

    public async Task<bool> Update(Player player, SqlConnection conn = null)
    {
        var sql = "update players set entryid = @EntryId, playername = @PlayerName, teamname = @Teamname where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, player);
        var success = result > 0;
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return success;
    }

    public async Task<bool> ReplacePlayersByLeagueId(Player[] players, int leagueId, int seasonId)
    {
        var existItems = await GetAllByLeagueId(leagueId);
        var allPlayers = await GetAllBySeasonId(seasonId);
        var newIds = players.Select(x => x.EntryId).Except(existItems.Select(x => x.EntryId));
        var oldIds = existItems.Select(x => x.EntryId).Except(players.Select(x => x.EntryId));
        var sameIds = existItems.Select(x => x.EntryId).Intersect(players.Select(x => x.EntryId));

        using var conn = await OpenConnection();

        foreach (var id in newIds)
        {
            // if the player already exists, the only add it into the players_in_leagues table
            if (allPlayers.Any(x => x.EntryId == id))
            {
                var existPlayer = allPlayers.First(x => x.EntryId == id);

                var playerInLeague = new PlayerInLeague() { PlayerId = existPlayer.Id, LeagueId = leagueId };
                await new PlayerInLeagueRepository(Configuration).Insert(playerInLeague, conn);

                existItems = existItems.Append(existPlayer).ToArray();
                sameIds = sameIds.Append(existPlayer.EntryId);
                continue;
            }

            var player = players.FirstOrDefault(x => x.EntryId == id);
            if (player == null) continue;

            var result = await Insert(player, conn);
            if (result <= 0)
                return false;
        }

        foreach (var id in oldIds)
        {
            // if there are still other players with the same entry id, then only delete from the players_in_leagues table and not the players table
            if (allPlayers.Count(x => x.EntryId == id) > 1)
            {
                var existPlayer = allPlayers.First(x => x.EntryId == id);

                await new PlayerInLeagueRepository(Configuration).DeleteByPlayerId(existPlayer.Id);
                continue;
            }

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
