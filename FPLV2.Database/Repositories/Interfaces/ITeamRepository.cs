using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface ITeamRepository
{
    Task<Team> GetById(int id);
    Task<Team[]> GetAll();
    Task<Team[]> GetAllBySeasonId(int seasonId);
    Task<bool> DeleteById(int id);
    Task DeleteAll();
    Task<int> Insert(Team team);
    Task<bool> Update(Team team);
}
