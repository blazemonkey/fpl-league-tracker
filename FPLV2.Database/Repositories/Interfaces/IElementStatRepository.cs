using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IElementStatRepository
{
    Task<ElementStat[]> GetAllBySeasonId(int seasonId);
    Task<int> Insert(ElementStat elementStat);
    Task<bool> Update(ElementStat elementStat);
}
