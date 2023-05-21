using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FPLV2.Database.Repositories;

public class ChartRepository : BaseRepository, IChartRepository
{
    public ChartRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<Chart[]> GetAll()
    {
        var sql = "select * from charts";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Chart>(sql);
        return result.ToArray();
    }

    public async Task<Chart> GetById(int id)
    {
        var sql = "select * from charts where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.QuerySingleOrDefaultAsync<Chart>(sql, new { Id = id });
        return result;
    }

    public async Task<IDictionary<string, LineChartPoint[]>> GetLineChart(string name, int seasonId, int leagueId)
    {
        var method = Assembly.GetAssembly(GetType()).GetType(GetType().FullName).GetMethod($"Get{name.Replace(" ", "")}");
        if (method == null)
            return null;

        var result = await (Task<IDictionary<string, LineChartPoint[]>>)method.Invoke(this, new object[] { seasonId, leagueId });
        return result;
    }

    public async Task<IDictionary<string, LineChartPoint[]>> GetGameweekTotalPointsHistory(int seasonId, int leagueId)
    {
        var sql = "select p.PlayerName, p.TeamName, po.Gameweek, po.Total as Points from leagues l join players_in_leagues pil on l.Id = pil.LeagueId join players p on pil.PlayerId = p.Id join points po on p.Id = po.PlayerId  join seasons s on l.SeasonId = s.Id where l.LeagueId = @LeagueId and s.Id = @SeasonId and po.Gameweek >= l.StartEvent order by p.Id, po.Gameweek";
        using var conn = await OpenConnection();
        var points = await conn.QueryAsync<PointsHistory>(sql, new { SeasonId = seasonId, LeagueId = leagueId }); // note this is the Id column in the Leagues table, not LeagueId
        
        var results = points.GroupBy(x => x.TeamName).ToDictionary(x => x.Key, x => x.Select(x => new LineChartPoint() { X = x.Gameweek, Y = x.Points}).ToArray());
        return results;
    }
}
