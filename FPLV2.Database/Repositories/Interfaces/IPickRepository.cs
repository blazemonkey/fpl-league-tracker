using FPLV2.Database.Models;
using Microsoft.Data.SqlClient;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IPickRepository
{
    Task<Pick[]> GetAllByPlayerId(int playerId);
    Task<Pick[]> GetAllByPlayerId(int playerId, int gameweek);
    Task<int> GetLatestGameweekByPlayerId(int playerId);
    Task<bool> DeleteById(int id, SqlConnection conn = null);
    Task<int> Insert(Pick pick, SqlConnection conn = null);
    Task<bool> Update(Pick pick, SqlConnection conn = null);
    Task<bool> ReplacePicksByPlayerId(Pick[] picks, int playerId, int gameweek);
}
