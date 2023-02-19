using FPLV2.Updater.Api;
using FPLV2.Updater.Models;
using System.Text.Json;

namespace FPLV2.UnitTests.UpdaterTests;

/// <summary>
/// Tests the corresponding StandingsApi class
/// </summary>
[TestClass]
public class PicksTests : UpdaterTests
{
    /// <summary>
    /// Gets the request url to be matched against
    /// </summary>
    protected override string RequestUrl => "entry/{0}/event/{1}/picks/";

    /// <summary>
    /// Gets the Sample Data json file name
    /// </summary>
    protected override string ResourceName => "picks";

    [TestMethod]
    public async Task RealDataTest()
    {
        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = entryId, PlayerName = "Fake Player", TeamName = "Fake Team" });

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 7, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 28, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 45, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 54, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 113, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 146, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 346, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 283, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 284, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 285, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 299, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 306, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 398, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 427, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 486, TeamId = teamId });

        var json = GetLiveDataJson();
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        var model = JsonSerializer.Deserialize<PickRoot>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        var picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        // grab a few random picks to check
        var rand = new Random();

        for (var i = 0; i < 3; i++)
        {
            var pick = model.Picks[rand.Next(model.Picks.Length)];
            var dbPick = picks.FirstOrDefault(x => x.PlayerId == playerId && x.Gameweek == BaseApi.CurrentGameweek && x.Position == pick.Position);
            Assert.IsNotNull(dbPick);
            AssertPick(dbPick, pick, playerId, elements);
        }
    }

    [TestMethod]
    public async Task PickNewTest()
    {
        var json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;

        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = entryId, PlayerName = "Fake Player", TeamName = "Fake Team" });

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 113, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        var picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        },
        {
          "element": 146,
          "position": 2,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;
        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 146, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        Assert.AreEqual(2, picks[1].Position);
        Assert.AreEqual(1, picks[1].Multiplier);
        Assert.AreEqual(playerId, picks[1].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 146)?.Id ?? 0, picks[1].ElementId);
    }

    [TestMethod]
    public async Task PickUpdateTest()
    {
        var json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;

        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = entryId, PlayerName = "Fake Player", TeamName = "Fake Team" });

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 113, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        var picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 2,
          "multiplier": 2,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;
        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 146, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(2, picks[0].Position);
        Assert.AreEqual(2, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);
    }


    [TestMethod]
    public async Task PickDeleteTest()
    {
        var json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        },
        {
          "element": 146,
          "position": 2,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;

        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = entryId, PlayerName = "Fake Player", TeamName = "Fake Team" });

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 113, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 146, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        var picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        Assert.AreEqual(2, picks[1].Position);
        Assert.AreEqual(1, picks[1].Multiplier);
        Assert.AreEqual(playerId, picks[1].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 146)?.Id ?? 0, picks[1].ElementId);

        json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;
        // insert the elements that are being referenced

        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);
    }

    [TestMethod]
    public async Task PickNewMultipleGameweekTest()
    {
        var jsonGw1 = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;
        var jsonGw2 = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 2,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;
        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 2;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = entryId, PlayerName = "Fake Player", TeamName = "Fake Team" });

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 113, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = jsonGw1 },
            new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 2), ResponseContent = jsonGw2 });

        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        var picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        Assert.AreEqual(1, picks[1].Position);
        Assert.AreEqual(2, picks[1].Multiplier);
        Assert.AreEqual(playerId, picks[1].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[1].ElementId);
    }

    [TestMethod]
    public async Task PickNewHasPreviousTest()
    {
        var json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;

        var leagueId = 124141;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { SeasonId = oldSeasonId, LeagueId = leagueId, Name = "Fake League" });
        var oldPlayerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { EntryId = 1234, LeagueId = oldLeagueId, PlayerName = "Fake Old Player", TeamName = "Fake Old Team" });
        var oldTeamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = oldSeasonId });
        var oldElementId = await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 1, TeamId = oldTeamId });
        await UnitOfWork.Picks.Insert(new Database.Models.Pick() { ElementId = oldElementId, Gameweek = 1, Multiplier = 3, PlayerId = oldPlayerId, Position = 10 });

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = entryId, PlayerName = "Fake Player", TeamName = "Fake Team" });

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 113, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        var picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        },
        {
          "element": 146,
          "position": 2,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;
        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 146, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        Assert.AreEqual(2, picks[1].Position);
        Assert.AreEqual(1, picks[1].Multiplier);
        Assert.AreEqual(playerId, picks[1].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 146)?.Id ?? 0, picks[1].ElementId);

        // make sure old data is fine
        elements = await UnitOfWork.Elements.GetAllBySeasonId(oldSeasonId);
        picks = await UnitOfWork.Picks.GetAllByPlayerId(oldPlayerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(10, picks[0].Position);
        Assert.AreEqual(3, picks[0].Multiplier);
        Assert.AreEqual(oldPlayerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 1)?.Id ?? 0, picks[0].ElementId);
    }

    [TestMethod]
    public async Task PickUpdateHasPreviousTest()
    {
        var json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;

        var leagueId = 124141;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { SeasonId = oldSeasonId, LeagueId = leagueId, Name = "Fake League" });
        var oldPlayerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { EntryId = 1234, LeagueId = oldLeagueId, PlayerName = "Fake Old Player", TeamName = "Fake Old Team" });
        var oldTeamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = oldSeasonId });
        var oldElementId = await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 1, TeamId = oldTeamId });
        await UnitOfWork.Picks.Insert(new Database.Models.Pick() { ElementId = oldElementId, Gameweek = 1, Multiplier = 3, PlayerId = oldPlayerId, Position = 10 });

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = entryId, PlayerName = "Fake Player", TeamName = "Fake Team" });

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 113, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        var picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 2,
          "multiplier": 2,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;
        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 146, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(2, picks[0].Position);
        Assert.AreEqual(2, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        // make sure old data is fine
        elements = await UnitOfWork.Elements.GetAllBySeasonId(oldSeasonId);
        picks = await UnitOfWork.Picks.GetAllByPlayerId(oldPlayerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(10, picks[0].Position);
        Assert.AreEqual(3, picks[0].Multiplier);
        Assert.AreEqual(oldPlayerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 1)?.Id ?? 0, picks[0].ElementId);
    }


    [TestMethod]
    public async Task PickDeleteHasPreviousTest()
    {
        var json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        },
        {
          "element": 146,
          "position": 2,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;

        var leagueId = 124141;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { SeasonId = oldSeasonId, LeagueId = leagueId, Name = "Fake League" });
        var oldPlayerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { EntryId = 1234, LeagueId = oldLeagueId, PlayerName = "Fake Old Player", TeamName = "Fake Old Team" });
        var oldTeamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = oldSeasonId });
        var oldElementId = await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 1, TeamId = oldTeamId });
        await UnitOfWork.Picks.Insert(new Database.Models.Pick() { ElementId = oldElementId, Gameweek = 1, Multiplier = 3, PlayerId = oldPlayerId, Position = 10 });

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });
        var entryId = 1234;
        var playerId = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = newLeagueId, EntryId = entryId, PlayerName = "Fake Player", TeamName = "Fake Team" });

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 113, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 146, TeamId = teamId });
        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        var picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(2, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        Assert.AreEqual(2, picks[1].Position);
        Assert.AreEqual(1, picks[1].Multiplier);
        Assert.AreEqual(playerId, picks[1].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 146)?.Id ?? 0, picks[1].ElementId);

        json = $$"""
      {
      "active_chip": null,
      "automatic_subs": [

      ],
      "entry_history": {
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
      "picks": [
        {
          "element": 113,
          "position": 1,
          "multiplier": 1,
          "is_captain": false,
          "is_vice_captain": false
        }
      ]
    }
    """;
        // insert the elements that are being referenced

        await ExecuteApi<PicksApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, entryId, 1), ResponseContent = json });

        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        picks = await UnitOfWork.Picks.GetAllByPlayerId(playerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(1, picks[0].Position);
        Assert.AreEqual(1, picks[0].Multiplier);
        Assert.AreEqual(playerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 113)?.Id ?? 0, picks[0].ElementId);

        // make sure old data is fine
        elements = await UnitOfWork.Elements.GetAllBySeasonId(oldSeasonId);
        picks = await UnitOfWork.Picks.GetAllByPlayerId(oldPlayerId);
        Assert.AreEqual(1, picks.Length);
        Assert.AreEqual(10, picks[0].Position);
        Assert.AreEqual(3, picks[0].Multiplier);
        Assert.AreEqual(oldPlayerId, picks[0].PlayerId);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 1)?.Id ?? 0, picks[0].ElementId);
    }

    /// <summary>
    /// Assert the Database model and the API model for the Pick is correct
    /// </summary>
    /// <param name="dbPick">Pick model stored in Database</param>
    /// <param name="pick">Pick model returned from API</param>
    /// <param name="playerId">Id of the season</param>
    /// <param name="dbElements">Element models stored in Database</param>
    private static void AssertPick(Database.Models.Pick dbPick, Pick pick, int playerId, Database.Models.Element[] dbElements)
    {
        Assert.AreEqual(pick.Multiplier, dbPick.Multiplier);
        Assert.AreEqual(pick.Position, dbPick.Position);
        Assert.AreEqual(BaseApi.CurrentGameweek, dbPick.Gameweek);
        Assert.AreEqual(dbElements.FirstOrDefault(x => x.ElementId == pick.Element)?.Id ?? 0, dbPick.ElementId);
    }

}
