using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IElementRepository
{
    Task<Element> GetById(int id);
    Task<Element[]> GetAll();
    Task<Element[]> GetAllByTeamId(int teamId);
    Task<Element[]> GetAllBySeasonId(int seasonId);
    Task<bool> DeleteById(int id);
    Task DeleteAll();
    Task<int> Insert(Element element);
    Task<bool> Update(Element element);
}
