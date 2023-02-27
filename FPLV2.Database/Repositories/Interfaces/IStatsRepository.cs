using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IStatsRepository
{
    Task<Stats[]> GetAll();
    Task<List<IDictionary<string, object>>> GetMostPointsInAGameweek(int seasonId, int leagueId);
    Task<List<IDictionary<string, object>>> GetMostBenchPointsInAGameweek(int seasonId, int leagueId);
}
