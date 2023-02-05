using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface ILeagueRepository
{
    Task<League[]> GetAll();
    Task<League[]> GetAllBySeasonId(int seasonId);
    Task DeleteAll();
    Task<int> Insert(League league);
    Task<bool> Update(League league);
}
