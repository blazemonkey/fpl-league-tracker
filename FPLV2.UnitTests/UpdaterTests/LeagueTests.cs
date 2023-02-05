using FPLV2.Updater.Models;
using System.Text.Json;

namespace FPLV2.UnitTests.UpdaterTests;

[TestClass]
public class LeagueTests : UpdaterTests
{
    public const string League2022Json = $$"""
        {
          "new_entries": {
            "has_next": false,
            "page": 1,
            "results": []
          },
          "last_updated_data": "2023-01-18T23:22:51Z",
          "league": {
            "id": 124141,
            "name": "PSL",
            "created": "2022-07-06T10:28:40.793171Z",
            "closed": false,
            "max_entries": null,
            "league_type": "x",
            "scoring": "c",
            "admin_entry": 648605,
            "start_event": 1,
            "code_privacy": "p",
            "has_cup": true,
            "cup_league": null,
            "rank": null
          }
        }
     """;
    public const string League2023Json = $$"""
        {
          "new_entries": {
            "has_next": false,
            "page": 1,
            "results": []
          },
          "last_updated_data": "2024-01-18T23:22:51Z",
          "league": {
            "id": 124141,
            "name": "PSL",
            "created": "2023-07-06T10:28:40.793171Z",
            "closed": false,
            "max_entries": null,
            "league_type": "x",
            "scoring": "c",
            "admin_entry": 648605,
            "start_event": 1,
            "code_privacy": "p",
            "has_cup": true,
            "cup_league": null,
            "rank": null
          }
        }
     """;



    [TestMethod]
    public async Task RealDataTest()
    {
        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        var bsJson = EmbeddedResourceHelper.GetResourceFromJson("bootstrap-static");
        var leagueJson = EmbeddedResourceHelper.GetResourceFromJson("league");
        await ExecUpdaterFunction(
            new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = bsJson },
            new Models.MockHttpParameter() { RequestUrl = "leagues-classic/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = leagueJson }
            );

        var model = JsonSerializer.Deserialize<Standings>(leagueJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        var season = await UnitOfWork.Seasons.GetByYear("2022/23");
        Assert.IsNotNull(season);

        var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(season.Id);
        Assert.AreEqual(1, leagues.Length);

        var players = await UnitOfWork.Players.GetAllByLeagueId(leagueId);
        Assert.AreEqual(5, players.Length);

        // grab a few random players to check
        var rand = new Random();

        for (var i = 0; i < 3; i++)
        {
            var player = model.Results.Players[rand.Next(model.Results.Players.Length)];
            var dbPlayer = players.FirstOrDefault(x => x.EntryId == player.Entry && x.LeagueId == leagueId);
            Assert.IsNotNull(dbPlayer);
            AssertPlayer(dbPlayer, player, leagueId);
        }
    }

    [TestMethod]
    public async Task LeagueSameSeasonSameTest()
    {
        var leagueId = 124141;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        var bsJson = EmbeddedResourceHelper.GetResourceFromJson("bootstrap-static");
        await ExecUpdaterFunction(
            new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = bsJson },
            new Models.MockHttpParameter() { RequestUrl = "leagues-classic/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = League2022Json }
        );

        var season = await UnitOfWork.Seasons.GetByYear("2022/23");
        Assert.IsNotNull(season);

        var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(season.Id);
        Assert.AreEqual(1, leagues.Length);
        Assert.AreEqual("PSL", leagues[0].Name);
        Assert.AreEqual(leagueId, leagues[0].LeagueId);
        Assert.AreEqual(season.Id, leagues[0].SeasonId);

        // call again
        await ExecUpdaterFunction(
            new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = bsJson },
            new Models.MockHttpParameter() { RequestUrl = "leagues-classic/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = League2022Json }
        );

        season = await UnitOfWork.Seasons.GetByYear("2022/23");
        Assert.IsNotNull(season);

        leagues = await UnitOfWork.Leagues.GetAllBySeasonId(season.Id);
        Assert.AreEqual(1, leagues.Length);
        Assert.AreEqual("PSL", leagues[0].Name);
        Assert.AreEqual(leagueId, leagues[0].LeagueId);
        Assert.AreEqual(season.Id, leagues[0].SeasonId);
    }

    [TestMethod]
    public async Task LeagueSameSeasonDifferentTest()
    {
        var leagueId = 124141;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        var bsJson = EmbeddedResourceHelper.GetResourceFromJson("bootstrap-static");
        await ExecUpdaterFunction(
            new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = bsJson },
            new Models.MockHttpParameter() { RequestUrl = "leagues-classic/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = League2022Json }
        );

        var season = await UnitOfWork.Seasons.GetByYear("2022/23");
        Assert.IsNotNull(season);

        var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(season.Id);
        Assert.AreEqual(1, leagues.Length);
        Assert.AreEqual("PSL", leagues[0].Name);
        Assert.AreEqual(leagueId, leagues[0].LeagueId);
        Assert.AreEqual(season.Id, leagues[0].SeasonId);

        seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2023/24" });
        await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecUpdaterFunction(
            new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = bsJson },
            new Models.MockHttpParameter() { RequestUrl = "leagues-classic/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = League2023Json }
        );

        season = await UnitOfWork.Seasons.GetByYear("2023/24");
        Assert.IsNotNull(season);

        leagues = await UnitOfWork.Leagues.GetAllBySeasonId(season.Id);
        Assert.AreEqual(1, leagues.Length);
        Assert.AreEqual("PSL", leagues[0].Name);
        Assert.AreEqual(leagueId, leagues[0].LeagueId);
        Assert.AreEqual(season.Id, leagues[0].SeasonId);
    }
}
