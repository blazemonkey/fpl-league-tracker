using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface ISeasonRepository
{
    Task<Season> GetById(int id);
    Task<Season> GetByYear(string year);
    Task<Season[]> GetAll();
    Task DeleteAll();
    Task<int> Insert(Season season);
    Task<bool> Update(Season season);
}
