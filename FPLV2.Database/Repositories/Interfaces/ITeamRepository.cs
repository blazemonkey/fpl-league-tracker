using FPLV2.Database.Models;
using Microsoft.Data.SqlClient;

namespace FPLV2.Database.Repositories.Interfaces;

public interface ITeamRepository
{
    Task<Team> GetById(int id);
    Task<Team[]> GetAll();
    Task<Team[]> GetAllBySeasonId(int seasonId);
    Task<bool> DeleteById(int id, SqlConnection conn = null);
    Task DeleteAll();
    Task<int> Insert(Team team, SqlConnection conn = null);
    Task<bool> Update(Team team, SqlConnection conn = null);
    Task<bool> ReplaceTeamsBySeasonId(Team[] teams, int seasonId);
}
