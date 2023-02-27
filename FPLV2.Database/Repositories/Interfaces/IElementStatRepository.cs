using FPLV2.Database.Models;
using Microsoft.Data.SqlClient;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IElementStatRepository
{
    Task<ElementStat[]> GetAllBySeasonId(int seasonId);
    Task<ElementStat[]> GetAllBySeasonId(int seasonId, int gameweek);
    Task<int> GetLatestGameweekBySeasonId(int seasonId);
    Task<bool> DeleteById(int id, SqlConnection conn = null);
    Task DeleteAll();
    Task<int> Insert(ElementStat elementStat, SqlConnection conn = null);
    Task<bool> Update(ElementStat elementStat, SqlConnection conn = null);
    Task<bool> ReplaceElementStatsBySeasonId(ElementStat[] elementStats, int seasonId, int gameweek);
}
