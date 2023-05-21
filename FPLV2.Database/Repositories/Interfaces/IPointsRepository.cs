using FPLV2.Database.Models;
using Microsoft.Data.SqlClient;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IPointsRepository
{
    Task<Points[]> GetAllByPlayerId(int playerId);
    Task<Points[]> GetAllByPlayerId(int playerId, int gameweek);
    Task DeleteAll();
    Task<bool> DeleteById(int id, SqlConnection conn = null);
    Task<int> Insert(Points points, SqlConnection conn = null);
    Task<bool> Update(Points points, SqlConnection conn = null);
    Task<bool> ReplacePointsByPlayerId(Points[] points, int playerId);
}
