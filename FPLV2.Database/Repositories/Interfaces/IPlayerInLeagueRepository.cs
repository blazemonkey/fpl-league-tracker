using FPLV2.Database.Models;
using Microsoft.Data.SqlClient;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IPlayerInLeagueRepository
{
    Task<int> Insert(PlayerInLeague playerInLeague, SqlConnection conn = null);
    Task<bool> DeleteByPlayerId(int playerId, SqlConnection conn = null);
}
