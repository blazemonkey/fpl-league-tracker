using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLV2.Database.Repositories;

/// <summary>
/// Repository that represents the players_in_leagues table
/// </summary>
public class PlayerInLeagueRepository : BaseRepository, IPlayerInLeagueRepository
{
    /// <summary>
    /// Constructor for the PlayerInLeagueRepository
    /// </summary>
    /// <param name="configuration">Configurations used for Repositories</param>
    public PlayerInLeagueRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<int> Insert(PlayerInLeague playerInLeague, SqlConnection conn = null)
    {
        var sql = "insert into players_in_leagues (playerid, leagueid) output inserted.id values (@PlayerId, @LeagueId)";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteScalarAsync<int>(sql, playerInLeague);
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result;
    }

    public async Task<bool> DeleteByPlayerId(int playerId, SqlConnection conn = null)
    {
        var sql = "delete from players_in_leagues where playerid = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, new { Id = playerId });
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result > 0;
    }
}
