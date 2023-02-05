using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IPointsRepository
{
    Task<Points[]> GetAllByPlayerId(int playerId);
    Task<int> Insert(Points points);
    Task<bool> Update(Points points);
}
