using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

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

    public async Task<int> Insert(Player player)
    {
        var sql = "insert into players (leagueid, entryid, playername, teamname) output inserted.id values (@LeagueId, @EntryId, @PlayerName, @TeamName)";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteScalarAsync<int>(sql, player);
        return result;
    }

    public async Task<bool> Update(Player player)
    {
        var sql = "update players set leagueid = @LeagueId, entryid = @EntryId, playername = @PlayerName, teamname = @Teamname where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, player);
        var success = result > 0;
        return success;
    }
}
