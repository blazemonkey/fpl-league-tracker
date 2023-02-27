using FPLV2.Updater.Api;
using FPLV2.Updater.Models;
using System.Text.Json;

namespace FPLV2.UnitTests.UpdaterTests;

/// <summary>
/// Tests the corresponding HistoryApi class
/// </summary>
[TestClass]
public class HistoryTests : UpdaterTests
{
    /// <summary>
    /// Gets the request url to be matched against
    /// </summary>
    protected override string RequestUrl => "entry/{0}/history/";

    /// <summary>
    /// Gets the Sample Data json file name
    /// </summary>
    protected override string ResourceName => "history";

    [TestMethod]
    public async Task RealDataTest()
    {
        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = 1234, PlayerName = "Fake Player", TeamName = "Fake Team" });

        var json = GetLiveDataJson();
        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        var model = JsonSerializer.Deserialize<PointsRoot>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        var points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        // grab a few random points to check
        var rand = new Random();

        for (var i = 0; i < 3; i++)
        {
            var currentPoints = model.Current[rand.Next(model.Current.Length)];
            var dbPoints = points.FirstOrDefault(x => x.PlayerId == playerId && x.Gameweek == currentPoints.Event);
            Assert.IsNotNull(dbPoints);
            AssertPoints(dbPoints, currentPoints, playerId);
        }
    }

    [TestMethod]
    public async Task PointsNewTest()
    {
        var json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 59,
      "total_points": 59,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    }    
  ]
}
""";

        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = 1234, PlayerName = "Fake Player", TeamName = "Fake Team" });

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        var points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, points.Length);
        Assert.AreEqual(59, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(59, points[0].Total);

        json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 59,
      "total_points": 59,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    },
    {
      "event": 2,
      "points": 71,
      "total_points": 130,
      "rank": 1512043,
      "rank_sort": 1527242,
      "overall_rank": 1824558,
      "bank": 0,
      "value": 999,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 18
    }   
      ]
    }
""";

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, points.Length);
        Assert.AreEqual(59, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(59, points[0].Total);

        Assert.AreEqual(71, points[1].GameweekPoints);
        Assert.AreEqual(18, points[1].GameweekPointsOnBench);
        Assert.AreEqual(130, points[1].Total);
    }

    [TestMethod]
    public async Task PointsDeleteTest()
    {
        var json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 59,
      "total_points": 59,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    },
    {
      "event": 2,
      "points": 71,
      "total_points": 130,
      "rank": 1512043,
      "rank_sort": 1527242,
      "overall_rank": 1824558,
      "bank": 0,
      "value": 999,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 18
    }   
      ]
    }
""";

        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = 1234, PlayerName = "Fake Player", TeamName = "Fake Team" });

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        var points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, points.Length);
        Assert.AreEqual(59, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(59, points[0].Total);

        Assert.AreEqual(71, points[1].GameweekPoints);
        Assert.AreEqual(18, points[1].GameweekPointsOnBench);
        Assert.AreEqual(130, points[1].Total);

        json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 59,
      "total_points": 59,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    }   
      ]
    }
""";

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, points.Length);
        Assert.AreEqual(59, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(59, points[0].Total);
    }


    [TestMethod]
    public async Task PointsUpdateTest()
    {
        var json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 59,
      "total_points": 59,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    }    
  ]
}
""";

        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = 1234, PlayerName = "Fake Player", TeamName = "Fake Team" });

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        var points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, points.Length);
        Assert.AreEqual(59, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(59, points[0].Total);

        json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 159,
      "total_points": 159,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    },
    {
      "event": 2,
      "points": 71,
      "total_points": 230,
      "rank": 1512043,
      "rank_sort": 1527242,
      "overall_rank": 1824558,
      "bank": 0,
      "value": 999,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 18
    }   
      ]
    }
""";

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, points.Length);
        Assert.AreEqual(159, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(159, points[0].Total);

        Assert.AreEqual(71, points[1].GameweekPoints);
        Assert.AreEqual(18, points[1].GameweekPointsOnBench);
        Assert.AreEqual(230, points[1].Total);
    }

    [TestMethod]
    public async Task PointsNewHasPreviousTest()
    {
        var json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 59,
      "total_points": 59,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    }    
  ]
}
""";

        var leagueId = 124141;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { SeasonId = oldSeasonId, LeagueId = leagueId, Name = "Fake League" });
        var oldPlayerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { EntryId = 1234, LeagueId = oldLeagueId, PlayerName = "Fake Old Player", TeamName = "Fake Old Team" });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId, GameweekPoints = 100, Gameweek = 1, GameweekPointsOnBench = 3, Total = 100 });

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = 1234, PlayerName = "Fake Player", TeamName = "Fake Team" });

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        var points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, points.Length);
        Assert.AreEqual(59, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(59, points[0].Total);

        json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 59,
      "total_points": 59,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    },
    {
      "event": 2,
      "points": 71,
      "total_points": 130,
      "rank": 1512043,
      "rank_sort": 1527242,
      "overall_rank": 1824558,
      "bank": 0,
      "value": 999,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 18
    }   
      ]
    }
""";

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, points.Length);
        Assert.AreEqual(59, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(59, points[0].Total);

        Assert.AreEqual(71, points[1].GameweekPoints);
        Assert.AreEqual(18, points[1].GameweekPointsOnBench);
        Assert.AreEqual(130, points[1].Total);

        // make sure old data is fine
        points = await UnitOfWork.Points.GetAllByPlayerId(oldPlayerId);
        Assert.AreEqual(1, points.Length);
        Assert.AreEqual(100, points[0].GameweekPoints);
        Assert.AreEqual(3, points[0].GameweekPointsOnBench);
        Assert.AreEqual(100, points[0].Total);
    }

    [TestMethod]
    public async Task PointsDeleteHasPreviousTest()
    {
        var json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 59,
      "total_points": 59,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    },
    {
      "event": 2,
      "points": 71,
      "total_points": 130,
      "rank": 1512043,
      "rank_sort": 1527242,
      "overall_rank": 1824558,
      "bank": 0,
      "value": 999,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 18
    }   
      ]
    }
""";

        var leagueId = 124141;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { SeasonId = oldSeasonId, LeagueId = leagueId, Name = "Fake League" });
        var oldPlayerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { EntryId = 1234, LeagueId = oldLeagueId, PlayerName = "Fake Old Player", TeamName = "Fake Old Team" });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId, GameweekPoints = 100, Gameweek = 1, GameweekPointsOnBench = 3, Total = 100 });

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = 1234, PlayerName = "Fake Player", TeamName = "Fake Team" });

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        var points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, points.Length);
        Assert.AreEqual(59, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(59, points[0].Total);

        Assert.AreEqual(71, points[1].GameweekPoints);
        Assert.AreEqual(18, points[1].GameweekPointsOnBench);
        Assert.AreEqual(130, points[1].Total);

        // make sure old data is fine
        points = await UnitOfWork.Points.GetAllByPlayerId(oldPlayerId);
        Assert.AreEqual(1, points.Length);
        Assert.AreEqual(100, points[0].GameweekPoints);
        Assert.AreEqual(3, points[0].GameweekPointsOnBench);
        Assert.AreEqual(100, points[0].Total);

        json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 59,
      "total_points": 59,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    }   
      ]
    }
""";

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, points.Length);
        Assert.AreEqual(59, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(59, points[0].Total);
    }


    [TestMethod]
    public async Task PointsUpdateHasPreviousTest()
    {
        var json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 59,
      "total_points": 59,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    }    
  ]
}
""";

        var leagueId = 124141;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { SeasonId = oldSeasonId, LeagueId = leagueId, Name = "Fake League" });
        var oldPlayerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { EntryId = 1234, LeagueId = oldLeagueId, PlayerName = "Fake Old Player", TeamName = "Fake Old Team" });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId, GameweekPoints = 100, Gameweek = 1, GameweekPointsOnBench = 3, Total = 100 });

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = 1234, PlayerName = "Fake Player", TeamName = "Fake Team" });

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        var points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, points.Length);
        Assert.AreEqual(59, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(59, points[0].Total);

        json = $$"""
{
  "current": [
    {
      "event": 1,
      "points": 159,
      "total_points": 159,
      "rank": 3699295,
      "rank_sort": 3739127,
      "overall_rank": 3699292,
      "bank": 0,
      "value": 1000,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 8
    },
    {
      "event": 2,
      "points": 71,
      "total_points": 230,
      "rank": 1512043,
      "rank_sort": 1527242,
      "overall_rank": 1824558,
      "bank": 0,
      "value": 999,
      "event_transfers": 0,
      "event_transfers_cost": 0,
      "points_on_bench": 18
    }   
      ]
    }
""";

        await ExecuteApi<HistoryApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId), ResponseContent = json });

        points = await UnitOfWork.Points.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, points.Length);
        Assert.AreEqual(159, points[0].GameweekPoints);
        Assert.AreEqual(8, points[0].GameweekPointsOnBench);
        Assert.AreEqual(159, points[0].Total);

        Assert.AreEqual(71, points[1].GameweekPoints);
        Assert.AreEqual(18, points[1].GameweekPointsOnBench);
        Assert.AreEqual(230, points[1].Total);

        // make sure old data is fine
        points = await UnitOfWork.Points.GetAllByPlayerId(oldPlayerId);
        Assert.AreEqual(1, points.Length);
        Assert.AreEqual(100, points[0].GameweekPoints);
        Assert.AreEqual(3, points[0].GameweekPointsOnBench);
        Assert.AreEqual(100, points[0].Total);
    }

    /// <summary>
    /// Assert the Database model and the API model for the Points is correct
    /// </summary>
    /// <param name="dbPoints">Pick model stored in Database</param>
    /// <param name="points">Pick model returned from API</param>
    /// <param name="playerId">Id of the season</param>
    private static void AssertPoints(Database.Models.Points dbPoints, Points points, int playerId)
    {
        Assert.AreEqual(points.Event, dbPoints.Gameweek);
        Assert.AreEqual(points.EventPoints, dbPoints.GameweekPoints);
        Assert.AreEqual(points.PointsOnBench, dbPoints.GameweekPointsOnBench);
        Assert.AreEqual(points.TotalPoints, dbPoints.Total);
    }
}
