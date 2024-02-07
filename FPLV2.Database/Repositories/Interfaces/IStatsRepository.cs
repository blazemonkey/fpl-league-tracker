using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface IStatsRepository
{
    Task<Stats[]> GetAll();
    Task<Stats> GetById(int id);
    Task<List<IDictionary<string, object>>> GetOverallStatsDetails(string name, int seasonId, int leagueId);
    Task<List<IDictionary<string, object>>> GetMostPointsInAGameweek(int seasonId, int leagueId);
    Task<List<IDictionary<string, object>>> GetMostBenchPointsInAGameweek(int seasonId, int leagueId);
    Task<List<IDictionary<string, object>>> GetMostCaptainPoints(int seasonId, int leagueId);
    Task<List<IDictionary<string, object>>> GetTeamStatsDetails(string name, int seasonId, int leagueId, int playerId);
}
