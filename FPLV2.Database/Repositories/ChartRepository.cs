using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Reflection;

namespace FPLV2.Database.Repositories;

public class ChartRepository : BaseRepository, IChartRepository
{
    public ChartRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<Chart[]> GetAll()
    {
        var sql = "select * from charts where active = 1";
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
        var points = await conn.QueryAsync<PointsHistory>(sql, new { SeasonId = seasonId, LeagueId = leagueId });
        
        var results = points.GroupBy(x => x.TeamName).ToDictionary(x => x.Key, x => x.Select(x => new LineChartPoint() { X = x.Gameweek, Y = x.Points}).ToArray());
        return results;
    }

    public async Task<IDictionary<string, LineChartPoint[]>> GetGameweekPointsHistory(int seasonId, int leagueId)
    {
        var sql = "select p.PlayerName, p.TeamName, po.Gameweek, po.GameweekPoints as Points from leagues l join players_in_leagues pil on l.Id = pil.LeagueId join players p on pil.PlayerId = p.Id join points po on p.Id = po.PlayerId  join seasons s on l.SeasonId = s.Id where l.LeagueId = @LeagueId and s.Id = @SeasonId and po.Gameweek >= l.StartEvent order by p.Id, po.Gameweek";
        using var conn = await OpenConnection();
        var points = await conn.QueryAsync<PointsHistory>(sql, new { SeasonId = seasonId, LeagueId = leagueId });

        var results = points.GroupBy(x => x.TeamName).ToDictionary(x => x.Key, x => x.Select(x => new LineChartPoint() { X = x.Gameweek, Y = x.Points }).ToArray());
        return results;
    }

    public async Task<IDictionary<string, LineChartPoint[]>> GetGameweekTotalBenchPointsHistory(int seasonId, int leagueId)
    {
        var sql = "select p.PlayerName, p.TeamName, po.Gameweek, po.Total as Points from leagues l join players_in_leagues pil on l.Id = pil.LeagueId join players p on pil.PlayerId = p.Id join points po on p.Id = po.PlayerId  join seasons s on l.SeasonId = s.Id where l.LeagueId = @LeagueId and s.Id = @SeasonId and po.Gameweek >= l.StartEvent order by p.Id, po.Gameweek";
        using var conn = await OpenConnection();
        var points = await conn.QueryAsync<PointsHistory>(sql, new { SeasonId = seasonId, LeagueId = leagueId });

        var results = points.GroupBy(x => x.TeamName).ToDictionary(x => x.Key, x => x.Select(x => new LineChartPoint() { X = x.Gameweek, Y = x.Points }).ToArray());
        return results;
    }

    public async Task<IDictionary<string, LineChartPoint[]>> GetGameweekBenchPointsHistory(int seasonId, int leagueId)
    {
        var sql = "select p.PlayerName, p.TeamName, po.Gameweek, po.GameweekPointsOnBench as Points from leagues l join players_in_leagues pil on l.Id = pil.LeagueId join players p on pil.PlayerId = p.Id join points po on p.Id = po.PlayerId  join seasons s on l.SeasonId = s.Id where l.LeagueId = @LeagueId and s.Id = @SeasonId and po.Gameweek >= l.StartEvent order by p.Id, po.Gameweek";
        using var conn = await OpenConnection();
        var points = await conn.QueryAsync<PointsHistory>(sql, new { SeasonId = seasonId, LeagueId = leagueId });

        var results = points.GroupBy(x => x.TeamName).ToDictionary(x => x.Key, x => x.Select(x => new LineChartPoint() { X = x.Gameweek, Y = x.Points }).ToArray());
        return results;
    }

    public async Task<IDictionary<string, LineChartPoint[]>> GetStandingsHistory(int seasonId, int leagueId)
    {
        var sql = "select p.PlayerName, p.TeamName, po.Gameweek, po.Total as Points from leagues l join players_in_leagues pil on l.Id = pil.LeagueId join players p on pil.PlayerId = p.Id join points po on p.Id = po.PlayerId  join seasons s on l.SeasonId = s.Id where l.LeagueId = @LeagueId and s.Id = @SeasonId and po.Gameweek >= l.StartEvent order by p.Id, po.Gameweek";
        using var conn = await OpenConnection();
        var points = await conn.QueryAsync<PointsHistory>(sql, new { SeasonId = seasonId, LeagueId = leagueId });

        var minGameweek = points.Min(x => x.Gameweek);
        var maxGameweek = points.Max(x => x.Gameweek);
        var standings = new List<StandingsHistory>();
        for (var i = minGameweek; i <= maxGameweek; i++)
        {
            var gw = points.Where(x => x.Gameweek == i).OrderByDescending(x => x.Points).ToArray();
            var gwGrouped = gw.GroupBy(x => x.Points);
            var st = gw.Select(x => new StandingsHistory() { Gameweek = x.Gameweek, TeamName = x.TeamName, Ranking = gw.Select(x => x.TeamName).ToList().IndexOf(x.TeamName) + 1 });
            standings.AddRange(st);
        }

        var results = standings.GroupBy(x => x.TeamName).ToDictionary(x => x.Key, x => x.Select(x => new LineChartPoint() { X = x.Gameweek, Y = x.Ranking * -1 }).ToArray());
        return results;
    }

    public async Task<PointsChartGroupedData[]> GetPointsChart(int seasonId, int leagueId, PointsChartOptions options)
    {
        var sql = "select e.Id, t.Code as TeamCode, e.ElementType, e.FirstName, e.SecondName, e.WebName, es.Gameweek, es.TotalPoints from elements e join elements_stats es on e.Id = es.ElementId join teams t on e.TeamId = t.Id where t.SeasonId = @SeasonId order by e.TeamId, e.Code, es.Gameweek";
        using var conn = await OpenConnection();
        var elements = await conn.QueryAsync<PointsChartElement>(sql, new { SeasonId = seasonId }, commandTimeout: 300);

        sql = "select pks.* from picks pks join players p on pks.PlayerId = p.Id join players_in_leagues pil on p.Id = pil.PlayerId join leagues l on pil.LeagueId = l.Id where l.SeasonId = @SeasonId and l.LeagueId = @LeagueId";
        var picks = await conn.QueryAsync<Pick>(sql, new { SeasonId = seasonId, LeagueId = leagueId }, commandTimeout: 300);

        foreach (var e in elements)
        {
            e.Picks = picks.Where(x => options.PlayerIds.Contains(x.PlayerId) && x.ElementId == e.Id && x.Gameweek == e.Gameweek && (options.ShowCaptainsOnly == false || x.Multiplier >= 2)).ToArray();
        }

        var result = elements.GroupBy(x => new { x.Id, x.TeamCode, x.ElementType, x.FirstName, x.SecondName, x.WebName }).Select(e => new PointsChartGroupedData()
        {
            Id = e.Key.Id,
            TeamCode = e.Key.TeamCode,
            ElementType = e.Key.ElementType,
            FirstName = e.Key.FirstName,
            SecondName = e.Key.SecondName,
            WebName = e.Key.WebName,
            Values = e.Select(item => new PointsChartValueData { Gameweek = item.Gameweek, Points = item.TotalPoints, Picks = item.Picks }).ToList()
        }).ToArray();

        if (options.IgnoreElementsWithNoPicks || options.ShowCaptainsOnly)
            result = result.Where(x => x.Values.Any(z => z.Picks.Any())).ToArray();

        if (options.ElementType > 0)
            result = result.Where(x => x.ElementType == options.ElementType).ToArray();        
        
        foreach (var r in result)
        {
            r.TotalPoints = r.Values.Sum(x => x.Points);
        }

        return result.OrderByDescending(x => x.TotalPoints).ThenBy(x => x.TeamCode).ThenBy(x => x.WebName).ToArray();
    }
}
