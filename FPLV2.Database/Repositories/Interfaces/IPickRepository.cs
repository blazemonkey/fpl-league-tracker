using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IPickRepository
{
    Task<Pick[]> GetAllByPlayerId(int playerId);
    Task<int> Insert(Pick pick);
    Task<bool> Update(Pick pick);
}
