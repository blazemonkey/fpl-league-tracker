using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IPlayerRepository
{
    Task<Player> Get(int leagueId, int entryId);
    Task<Player[]> GetAll();
    Task<Player[]> GetAllByLeagueId(int leagueId);
    Task<int> Insert(Player player);
    Task<bool> Update(Player player);
}
