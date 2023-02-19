using FPLV2.Database.Models;
using Microsoft.Data.SqlClient;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IPlayerRepository
{
    Task<Player> Get(int leagueId, int entryId);
    Task<Player[]> GetAll();
    Task<Player[]> GetAllByLeagueId(int leagueId);
    Task<bool> DeleteById(int id, SqlConnection conn = null);
    Task<int> Insert(Player player, SqlConnection conn = null);
    Task<bool> Update(Player player, SqlConnection conn = null);
    Task<bool> ReplacePlayersByLeagueId(Player[] players, int leagueId);
}
