using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IChartRepository
{
    Task<Chart[]> GetAll();
    Task<Chart> GetById(int id);
    Task<IDictionary<string, LineChartPoint[]>> GetLineChart(string name, int seasonId, int leagueId);
    Task<IDictionary<string, LineChartPoint[]>> GetGameweekTotalPointsHistory(int seasonId, int leagueId);
}
