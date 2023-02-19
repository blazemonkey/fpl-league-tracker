using FPLV2.Database.Models;
using Microsoft.Data.SqlClient;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IElementRepository
{
    Task<Element> GetById(int id);
    Task<Element[]> GetAll();
    Task<Element[]> GetAllByTeamId(int teamId);
    Task<Element[]> GetAllBySeasonId(int seasonId);
    Task<bool> DeleteById(int id, SqlConnection conn = null);
    Task DeleteAll();
    Task<int> Insert(Element element, SqlConnection conn = null);
    Task<bool> Update(Element element, SqlConnection conn = null);
    Task<bool> ReplaceElementsBySeasonId(Element[] elements, int seasonId);
}
