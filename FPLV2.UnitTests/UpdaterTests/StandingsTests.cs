using FPLV2.Updater.Api;
using FPLV2.Updater.Models;
using System.Text.Json;

namespace FPLV2.UnitTests.UpdaterTests;

/// <summary>
/// Tests the corresponding StandingsApi class
/// </summary>
[TestClass]
public class StandingsTests : UpdaterTests
{
    /// <summary>
    /// Gets the request url to be matched against
    /// </summary>
    protected override string RequestUrl => "leagues-classic/{0}/standings/?page_standings={1}";

    /// <summary>
    /// Gets the Sample Data json file name
    /// </summary>
    protected override string ResourceName => "standings";

    [TestMethod]
    public async Task RealDataTest()
    {
        var leagueId = 124141;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        var json = GetLiveDataJson();
        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        var model = JsonSerializer.Deserialize<Standings>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        var season = await UnitOfWork.Seasons.GetByYear("2022/23");
        Assert.IsNotNull(season);

        var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(season.Id);
        Assert.AreEqual(1, leagues.Length);

        var players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(5, players.Length);

        // grab a few random players to check
        var rand = new Random();

        for (var i = 0; i < 3; i++)
        {
            var player = model.Results.Players[rand.Next(model.Results.Players.Length)];
            var dbPlayer = players.FirstOrDefault(x => x.EntryId == player.Entry);
            Assert.IsNotNull(dbPlayer);
            AssertPlayer(dbPlayer, player, newLeagueId);
        }
    }

    [TestMethod]
    public async Task NoSeasonIdSetTest()
    {
        var leagueId = 124141;
        BaseApi.SeasonId = 0;

        var json = GetLiveDataJson();
        var success = await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });
        Assert.IsFalse(success);
    }
    
    [TestMethod]
    public async Task LeagueUpdateTest()
    {
        var leagueId = 124141;

        var json = $$"""
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

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });
        
        var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, leagues.Length);
        Assert.AreEqual("PSL", leagues[0].Name);
        Assert.AreEqual(leagueId, leagues[0].LeagueId);
        Assert.AreEqual(seasonId, leagues[0].SeasonId);

        json = $$"""
        {
          "new_entries": {
            "has_next": false,
            "page": 1,
            "results": []
          },
          "last_updated_data": "2023-01-18T23:22:51Z",
          "league": {
            "id": 124141,
            "name": "Updated PSL",
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
        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });
        
        leagues = await UnitOfWork.Leagues.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, leagues.Length);
        Assert.AreEqual("Updated PSL", leagues[0].Name);
        Assert.AreEqual(leagueId, leagues[0].LeagueId);
        Assert.AreEqual(seasonId, leagues[0].SeasonId);
    }

    [TestMethod]
    public async Task LeagueHasPreviousTest()
    {
        var leagueId = 124141;

        var json = $$"""
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

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        await UnitOfWork.Leagues.Insert(new Database.Models.League() { SeasonId = oldSeasonId, LeagueId = leagueId, Name = "Fake League" });

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, leagues.Length);
        Assert.AreEqual("PSL", leagues[0].Name);
        Assert.AreEqual(leagueId, leagues[0].LeagueId);
        Assert.AreEqual(seasonId, leagues[0].SeasonId);

        // make sure old data is fine
        leagues = await UnitOfWork.Leagues.GetAllBySeasonId(oldSeasonId);
        Assert.AreEqual(1, leagues.Length);
        Assert.AreEqual("Fake League", leagues[0].Name);
        Assert.AreEqual(leagueId, leagues[0].LeagueId);
        Assert.AreEqual(oldSeasonId, leagues[0].SeasonId);
    }

    [TestMethod]
    public async Task PlayerNewTest()
    {
        var leagueId = 124141;

        var json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             }
           ]
         }
        }
     """;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        var players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             },
             {
               "id": 7517407,
               "event_total": 44,
               "player_name": "G F",
               "rank": 5,
               "last_rank": 5,
               "rank_sort": 5,
               "total": 1047,
               "entry": 1414979,
               "entry_name": "Liverpole P1 FC"
             }
           ]
         }
        }
     """;
        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(2, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        Assert.AreEqual(1414979, players[1].EntryId);
        Assert.AreEqual("G F", players[1].PlayerName);
        Assert.AreEqual("Liverpole P1 FC", players[1].TeamName);
    }

    [TestMethod]
    public async Task PlayerSameTest()
    {
        var leagueId = 124141;

        var json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             }
           ]
         }
        }
     """;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        var players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        // call again       
        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);
    }

    [TestMethod]
    public async Task PlayerUpdateTest()
    {
        var leagueId = 124141;

        var json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             }
           ]
         }
        }
     """;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        var players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C A",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             }
           ]
         }
        }
     """;
        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C A", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);
    }

    [TestMethod]
    public async Task PlayerDeleteTest()
    {
        var leagueId = 124141;

        var json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             },
             {
               "id": 7517407,
               "event_total": 44,
               "player_name": "G F",
               "rank": 5,
               "last_rank": 5,
               "rank_sort": 5,
               "total": 1047,
               "entry": 1414979,
               "entry_name": "Liverpole P1 FC"
             }
           ]
         }
        }
     """;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        var players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(2, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        Assert.AreEqual(1414979, players[1].EntryId);
        Assert.AreEqual("G F", players[1].PlayerName);
        Assert.AreEqual("Liverpole P1 FC", players[1].TeamName);

        json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             }
           ]
         }
        }
     """;
        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);
    }

    [TestMethod]
    public async Task PlayerNewHasPreviousTest()
    {
        var leagueId = 124141;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { SeasonId = oldSeasonId, LeagueId = leagueId, Name = "Fake League" });
        await UnitOfWork.Players.Insert(new Database.Models.Player() { EntryId = 1234, LeagueId = oldLeagueId, PlayerName = "Fake Player", TeamName = "Fake Team" });

        var json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             }
           ]
         }
        }
     """;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        var players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             },
             {
               "id": 7517407,
               "event_total": 44,
               "player_name": "G F",
               "rank": 5,
               "last_rank": 5,
               "rank_sort": 5,
               "total": 1047,
               "entry": 1414979,
               "entry_name": "Liverpole P1 FC"
             }
           ]
         }
        }
     """;
        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(2, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        Assert.AreEqual(1414979, players[1].EntryId);
        Assert.AreEqual("G F", players[1].PlayerName);
        Assert.AreEqual("Liverpole P1 FC", players[1].TeamName);

        // make sure old data is fine
        players = await UnitOfWork.Players.GetAllByLeagueId(oldLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(1234, players[0].EntryId);
        Assert.AreEqual("Fake Player", players[0].PlayerName);
        Assert.AreEqual("Fake Team", players[0].TeamName);
    }

    [TestMethod]
    public async Task PlayerDeleteHasPreviousTest()
    {
        var leagueId = 124141;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { SeasonId = oldSeasonId, LeagueId = leagueId, Name = "Fake League" });
        await UnitOfWork.Players.Insert(new Database.Models.Player() { EntryId = 1234, LeagueId = oldLeagueId, PlayerName = "Fake Player", TeamName = "Fake Team" });

        var json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             },
             {
               "id": 7517407,
               "event_total": 44,
               "player_name": "G F",
               "rank": 5,
               "last_rank": 5,
               "rank_sort": 5,
               "total": 1047,
               "entry": 1414979,
               "entry_name": "Liverpole P1 FC"
             }
           ]
         }
        }
     """;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        var players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(2, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        Assert.AreEqual(1414979, players[1].EntryId);
        Assert.AreEqual("G F", players[1].PlayerName);
        Assert.AreEqual("Liverpole P1 FC", players[1].TeamName);

        json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             }
           ]
         }
        }
     """;
        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        // make sure old data is fine
        players = await UnitOfWork.Players.GetAllByLeagueId(oldLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(1234, players[0].EntryId);
        Assert.AreEqual("Fake Player", players[0].PlayerName);
        Assert.AreEqual("Fake Team", players[0].TeamName);
    }

    [TestMethod]
    public async Task PlayerMultiplePageTest()
    {
        var leagueId = 124141;

        var jsonPage1 = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": true,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             }
           ]
         }
        }
     """;
        var jsonPage2 = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 2,
           "results": [
             {
               "id": 7517407,
               "event_total": 44,
               "player_name": "G F",
               "rank": 5,
               "last_rank": 5,
               "rank_sort": 5,
               "total": 1047,
               "entry": 1414979,
               "entry_name": "Liverpole P1 FC"
             }
           ]
         }
        }
     """;
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = jsonPage1 },
            new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 2), ResponseContent = jsonPage2 });

        var players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(2, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        Assert.AreEqual(1414979, players[1].EntryId);
        Assert.AreEqual("G F", players[1].PlayerName);
        Assert.AreEqual("Liverpole P1 FC", players[1].TeamName);
    }

    [TestMethod]
    public async Task PlayerExistsInDifferentLeagueNewTest()
    {
        var leagueId = 124141;
        var leagueId2 = 224141;

        var json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             }
           ]
         }
        }
     """;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });       

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        var players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern", players[0].TeamName);

        var json2 = $$"""
        {
          "new_entries": {
            "has_next": false,
            "page": 1,
            "results": []
          },
          "last_updated_data": "2023-01-18T23:22:51Z",
          "league": {
            "id": 224141,
            "name": "PSL",
            "created": "2022-07-06T10:28:40.793171Z",
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C A",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern 2"
             }
           ]
         }
        }
     """;
        var newLeagueId2 = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId2, SeasonId = seasonId, Name = "PSL 2" });
        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json }, new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId2, 1), ResponseContent = json2 });

        players = await UnitOfWork.Players.GetAllByLeagueId(newLeagueId2);
        Assert.AreEqual(1, players.Length);
        Assert.AreEqual(648605, players[0].EntryId);
        Assert.AreEqual("K C A", players[0].PlayerName);
        Assert.AreEqual("When Mane met Bayern 2", players[0].TeamName);

        var totalPlayers = await UnitOfWork.Players.GetAll();
        Assert.AreEqual(1, totalPlayers.Length);
    }

    [TestMethod]
    public async Task PlayerExistsInDifferentLeagueDeleteTest()
    {
        var leagueId = 124141;
        var leagueId2 = 224141;

        var json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             },
             {
               "id": 7517407,
               "event_total": 44,
               "player_name": "G F",
               "rank": 5,
               "last_rank": 5,
               "rank_sort": 5,
               "total": 1047,
               "entry": 1414979,
               "entry_name": "Liverpole P1 FC"
             }
           ]
         }
        }
     """;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        var newLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId, SeasonId = seasonId, Name = "PSL" });

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json });

        var json2 = $$"""
        {
          "new_entries": {
            "has_next": false,
            "page": 1,
            "results": []
          },
          "last_updated_data": "2023-01-18T23:22:51Z",
          "league": {
            "id": 224141,
            "name": "PSL",
            "created": "2022-07-06T10:28:40.793171Z",
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C A",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern 2"
             },
             {
               "id": 7517407,
               "event_total": 44,
               "player_name": "G F",
               "rank": 5,
               "last_rank": 5,
               "rank_sort": 5,
               "total": 1047,
               "entry": 1414979,
               "entry_name": "Liverpole P1 FC"
             }
           ]
         }
        }
     """;
        var newLeagueId2 = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = leagueId2, SeasonId = seasonId, Name = "PSL 2" });
        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json }, new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId2, 1), ResponseContent = json2 });

        var totalPlayers = await UnitOfWork.Players.GetAll();
        Assert.AreEqual(2, totalPlayers.Length);


        json = $$"""
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
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern"
             }
           ]
         }
        }
     """;

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json }, new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId2, 1), ResponseContent = json2 });

        totalPlayers = await UnitOfWork.Players.GetAll();
        Assert.AreEqual(2, totalPlayers.Length);

        json2 = $$"""
        {
          "new_entries": {
            "has_next": false,
            "page": 1,
            "results": []
          },
          "last_updated_data": "2023-01-18T23:22:51Z",
          "league": {
            "id": 224141,
            "name": "PSL",
            "created": "2022-07-06T10:28:40.793171Z",
            "league_type": "x"
          },
         "standings": {
           "has_next": false,
           "page": 1,
           "results": [
             {
               "id": 3355895,
               "event_total": 65,
               "player_name": "K C A",
               "rank": 1,
               "last_rank": 1,
               "rank_sort": 1,
               "total": 1145,
               "entry": 648605,
               "entry_name": "When Mane met Bayern 2"
             }
           ]
         }
        }
     """;

        await ExecuteApi<StandingsApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId, 1), ResponseContent = json }, new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, leagueId2, 1), ResponseContent = json2 });

        totalPlayers = await UnitOfWork.Players.GetAll();
        Assert.AreEqual(1, totalPlayers.Length);
    }
}
