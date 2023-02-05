using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

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

    public async Task<int> Insert(Pick pick)
    {
        var sql = "insert into picks (playerId, gameweek, elementId, multiplier, position) output inserted.id values (@PlayerId, @Gameweek, @ElementId, @Multiplier, @Position)";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteScalarAsync<int>(sql, pick);
        return result;
    }

    public async Task<bool> Update(Pick pick)
    {
        var sql = "update picks set playerId = @PlayerId, gameweek = @Gameweek, elementId = @ElementId, multiplier = @Multiplier, position = @Position where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, pick);
        var success = result > 0;
        return success;
    }
}
