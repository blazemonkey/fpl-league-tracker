using FPLV2.Client.Models;
using FPLV2.Updater.Api;
using System.Text.Json;

namespace FPLV2.UnitTests.UpdaterTests;

/// <summary>
/// Tests the corresponding BootstrapStaticApi class
/// </summary>
[TestClass]
public class BootstrapStaticTests : UpdaterTests
{
    /// <summary>
    /// Gets the request url to be matched against
    /// </summary>
    protected override string RequestUrl => "bootstrap-static";

    /// <summary>
    /// Gets the Sample Data json file name
    /// </summary>
    protected override string ResourceName => "bootstrap-static";

    [TestMethod]
    public async Task RealDataTest()
    {
        var json = GetLiveDataJson();
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var model = JsonSerializer.Deserialize<BootstrapStatic>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        var seasons = await UnitOfWork.Seasons.GetAll();
        Assert.AreEqual(1, seasons.Length);
        Assert.AreEqual("2022/23", seasons[0].Year);

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(20, teams.Length);

        // grab a few random teams to check
        var rand = new Random();
        for (var i = 0; i < 5; i++)
        {
            var team = model.Teams[rand.Next(model.Teams.Length)];
            var dbTeam = teams.FirstOrDefault(x => x.TeamId == team.Id);
            Assert.IsNotNull(dbTeam);

            AssertTeam(dbTeam, team, seasons[0].Id);
        }

        var elements = await UnitOfWork.Elements.GetAll();

        // grab a few random elements to check
        for (var i = 0; i < 100; i++)
        {
            var element = model.Elements[rand.Next(model.Elements.Length)];
            var dbElement = elements.FirstOrDefault(x => x.ElementId == element.Id);
            Assert.IsNotNull(dbElement);

            var dbTeam = teams.FirstOrDefault(x => x.Id == dbElement.TeamId);
            Assert.IsNotNull(dbTeam);

            AssertElement(dbElement, element, dbTeam.Id);
        }
    }

    [TestMethod]
    public async Task SeasonNewTest()
    {    
        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 2,
              "name": "Gameweek 2",
              "deadline_time": "2022-08-13T10:00:00Z",
              "is_current": false
            },
            {
              "id": 3,
              "name": "Gameweek 3",
              "deadline_time": "2022-08-20T10:00:00Z",
              "is_current": false
            },
            {
              "id": 4,
              "name": "Gameweek 4",
              "deadline_time": "2022-08-27T10:00:00Z",
              "is_current": true
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": false
            }]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var seasons = await UnitOfWork.Seasons.GetAll();
        Assert.AreEqual(1, seasons.Length);
        Assert.AreEqual("2022/23", seasons[0].Year);

        json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2023-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 2,
              "name": "Gameweek 2",
              "deadline_time": "2023-08-13T10:00:00Z",
              "is_current": false
            },
            {
              "id": 3,
              "name": "Gameweek 3",
              "deadline_time": "2023-08-20T10:00:00Z",
              "is_current": false
            },
            {
              "id": 4,
              "name": "Gameweek 4",
              "deadline_time": "2023-08-27T10:00:00Z",
              "is_current": true
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2024-05-28T13:30:00Z",
              "is_current": false
            }]
        }
        """;
        // call with different json
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        seasons = await UnitOfWork.Seasons.GetAll();
        Assert.AreEqual(2, seasons.Length);
        Assert.AreEqual("2022/23", seasons[0].Year);
        Assert.AreEqual("2023/24", seasons[1].Year);
    }

    [TestMethod]
    public async Task SeasonSameTest()
    {
        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 2,
              "name": "Gameweek 2",
              "deadline_time": "2022-08-13T10:00:00Z",
              "is_current": false
            },
            {
              "id": 3,
              "name": "Gameweek 3",
              "deadline_time": "2022-08-20T10:00:00Z",
              "is_current": false
            },
            {
              "id": 4,
              "name": "Gameweek 4",
              "deadline_time": "2022-08-27T10:00:00Z",
              "is_current": true
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": false
            }]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var seasons = await UnitOfWork.Seasons.GetAll();
        Assert.AreEqual(1, seasons.Length);
        Assert.AreEqual("2022/23", seasons[0].Year);

        // call again
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        seasons = await UnitOfWork.Seasons.GetAll();
        Assert.AreEqual(1, seasons.Length);
        Assert.AreEqual("2022/23", seasons[0].Year);
    }

    [TestMethod]
    public async Task TeamNewTest()
    {
        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                },
                {
                  "code": 7,
                  "id": 2,
                  "name": "Aston Villa",
                  "short_name": "AVL"
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);

        json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                },
                {
                  "code": 7,
                  "id": 2,
                  "name": "Aston Villa",
                  "short_name": "AVL"
                },
                {
                  "code": 14,
                  "id": 12,
                  "name": "Liverpool",
                  "short_name": "LIV"
                },
                {
                  "code": 43,
                  "id": 13,
                  "name": "Man City",
                  "short_name": "MCI"
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(4, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);
        Assert.AreEqual("Liverpool", teams[2].Name);
        Assert.AreEqual(12, teams[2].TeamId);
        Assert.AreEqual("Man City", teams[3].Name);
        Assert.AreEqual(13, teams[3].TeamId);
    }

    [TestMethod]
    public async Task TeamSameTest()
    {
        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                },
                {
                  "code": 7,
                  "id": 2,
                  "name": "Aston Villa",
                  "short_name": "AVL"
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);

        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);
    }

    [TestMethod]
    public async Task TeamDeleteTest()
    {
        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                },
                {
                  "code": 7,
                  "id": 2,
                  "name": "Aston Villa",
                  "short_name": "AVL"
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);

        json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 14,
                  "id": 12,
                  "name": "Liverpool",
                  "short_name": "LIV"
                },
                {
                  "code": 43,
                  "id": 13,
                  "name": "Man City",
                  "short_name": "MCI"
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Liverpool", teams[0].Name);
        Assert.AreEqual(12, teams[0].TeamId);
        Assert.AreEqual("Man City", teams[1].Name);
        Assert.AreEqual(13, teams[1].TeamId);
    }

    [TestMethod]
    public async Task TeamNewHasPreviousTest()
    {
        // add some data for previous year
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "1998/99" });
        await UnitOfWork.Teams.Insert(new Database.Models.Team() { SeasonId = oldSeasonId, TeamId = 100, Name = "Fake Team", ShortName = "FT", Code = 100 });
        await UnitOfWork.Teams.Insert(new Database.Models.Team() { SeasonId = oldSeasonId, TeamId = 101, Name = "Fake Team 2", ShortName = "FT2", Code = 101 });
        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                },
                {
                  "code": 7,
                  "id": 2,
                  "name": "Aston Villa",
                  "short_name": "AVL"
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var season = await UnitOfWork.Seasons.GetByYear("2022/23");
        var teams = await UnitOfWork.Teams.GetAllBySeasonId(season.Id);
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);

        json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                },
                {
                  "code": 7,
                  "id": 2,
                  "name": "Aston Villa",
                  "short_name": "AVL"
                },
                {
                  "code": 14,
                  "id": 12,
                  "name": "Liverpool",
                  "short_name": "LIV"
                },
                {
                  "code": 43,
                  "id": 13,
                  "name": "Man City",
                  "short_name": "MCI"
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        teams = await UnitOfWork.Teams.GetAllBySeasonId(season.Id);
        Assert.AreEqual(4, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);
        Assert.AreEqual("Liverpool", teams[2].Name);
        Assert.AreEqual(12, teams[2].TeamId);
        Assert.AreEqual("Man City", teams[3].Name);
        Assert.AreEqual(13, teams[3].TeamId);

        // make sure old data is fine
        teams = await UnitOfWork.Teams.GetAllBySeasonId(oldSeasonId);
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Fake Team", teams[0].Name);
        Assert.AreEqual(100, teams[0].TeamId);
        Assert.AreEqual("Fake Team 2", teams[1].Name);
        Assert.AreEqual(101, teams[1].TeamId);
    }

    [TestMethod]
    public async Task TeamSameHasPreviousTest()
    {
        // add some data for previous year
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "1999/00" });
        await UnitOfWork.Teams.Insert(new Database.Models.Team() { SeasonId = oldSeasonId, TeamId = 100, Name = "Fake Team", ShortName = "FT", Code = 100 });
        await UnitOfWork.Teams.Insert(new Database.Models.Team() { SeasonId = oldSeasonId, TeamId = 101, Name = "Fake Team 2", ShortName = "FT2", Code = 101 });

        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                },
                {
                  "code": 7,
                  "id": 2,
                  "name": "Aston Villa",
                  "short_name": "AVL"
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var season = await UnitOfWork.Seasons.GetByYear("2022/23");
        var teams = await UnitOfWork.Teams.GetAllBySeasonId(season.Id);
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);

        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        teams = await UnitOfWork.Teams.GetAllBySeasonId(season.Id);
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);

        // make sure old data is fine
        teams = await UnitOfWork.Teams.GetAllBySeasonId(oldSeasonId);
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Fake Team", teams[0].Name);
        Assert.AreEqual(100, teams[0].TeamId);
        Assert.AreEqual("Fake Team 2", teams[1].Name);
        Assert.AreEqual(101, teams[1].TeamId);
    }

    [TestMethod]
    public async Task TeamDeleteHasPreviousTest()
    {
        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        await UnitOfWork.Teams.Insert(new Database.Models.Team() { SeasonId = oldSeasonId, TeamId = 1, Name = "Arsenal", ShortName = "ARS", Code = 3 });
        await UnitOfWork.Teams.Insert(new Database.Models.Team() { SeasonId = oldSeasonId, TeamId = 2, Name = "Aston Villa", ShortName = "AVL", Code = 7 });

        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                },
                {
                  "code": 7,
                  "id": 2,
                  "name": "Aston Villa",
                  "short_name": "AVL"
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var season = await UnitOfWork.Seasons.GetByYear("2022/23");
        var teams = await UnitOfWork.Teams.GetAllBySeasonId(season.Id);
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);

        json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 14,
                  "id": 12,
                  "name": "Liverpool",
                  "short_name": "LIV"
                },
                {
                  "code": 43,
                  "id": 13,
                  "name": "Man City",
                  "short_name": "MCI"
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        teams = await UnitOfWork.Teams.GetAllBySeasonId(season.Id);
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Liverpool", teams[0].Name);
        Assert.AreEqual(12, teams[0].TeamId);
        Assert.AreEqual("Man City", teams[1].Name);
        Assert.AreEqual(13, teams[1].TeamId);

        // make sure old data is fine
        teams = await UnitOfWork.Teams.GetAllBySeasonId(oldSeasonId);
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(1, teams[0].TeamId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(2, teams[1].TeamId);
    }


    [TestMethod]
    public async Task ElementNewTest()
    {
        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                }
            ],
            "elements": [
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 75,
                  "code": 223340,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 1,
                  "cost_change_start_fall": -1,
                  "dreamteam_count": 2,
                  "element_type": 3,
                  "ep_next": "6.5",
                  "ep_this": "4.5",
                  "event_points": 6,
                  "first_name": "Bukayo",
                  "form": "6.0",
                  "id": 13,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2023-01-13T15:00:07.612275Z",
                  "now_cost": 81,
                  "photo": "223340.jpg",
                  "points_per_game": "5.6",
                  "second_name": "Saka",
                  "selected_by_percent": "20.5",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 100,
                  "transfers_in": 2864171,
                  "transfers_in_event": 76897,
                  "transfers_out": 2891608,
                  "transfers_out_event": 55853,
                  "value_form": "0.7",
                  "value_season": "12.3",
                  "web_name": "Saka",
                  "minutes": 1534,
                  "goals_scored": 6,
                  "assists": 8,
                  "clean_sheets": 8,
                  "goals_conceded": 14,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 4,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 7,
                  "bps": 339,
                  "influence": "468.8",
                  "creativity": "567.8",
                  "threat": "517.0",
                  "ict_index": "155.4",
                  "starts": 18,
                  "expected_goals": "5.80730",
                  "expected_assists": "2.62779",
                  "expected_goal_involvements": "8.43509",
                  "expected_goals_conceded": "15.99530",
                  "influence_rank": 14,
                  "influence_rank_type": 3,
                  "creativity_rank": 7,
                  "creativity_rank_type": 5,
                  "threat_rank": 22,
                  "threat_rank_type": 13,
                  "ict_index_rank": 10,
                  "ict_index_rank_type": 6,
                  "corners_and_indirect_freekicks_order": 2,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": 1,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.34072,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.15417,
                  "expected_goal_involvements_per_90": 0.49489,
                  "expected_goals_conceded_per_90": 0.93845,
                  "goals_conceded_per_90": 0.82,
                  "now_cost_rank": 15,
                  "now_cost_rank_type": 7,
                  "form_rank": 15,
                  "form_rank_type": 7,
                  "points_per_game_rank": 14,
                  "points_per_game_rank_type": 6,
                  "selected_rank": 19,
                  "selected_rank_type": 8,
                  "starts_per_90": 1.05606,
                  "clean_sheets_per_90": 0.46936
                },
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 100,
                  "code": 206325,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 0,
                  "cost_change_start_fall": 0,
                  "dreamteam_count": 1,
                  "element_type": 2,
                  "ep_next": "3.7",
                  "ep_this": "3.2",
                  "event_points": 6,
                  "first_name": "Oleksandr",
                  "form": "3.2",
                  "id": 313,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2022-12-20T01:00:07.388064Z",
                  "now_cost": 50,
                  "photo": "206325.jpg",
                  "points_per_game": "4.5",
                  "second_name": "Zinchenko",
                  "selected_by_percent": "7.3",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 49,
                  "transfers_in": 1865110,
                  "transfers_in_event": 14505,
                  "transfers_out": 3077704,
                  "transfers_out_event": 5053,
                  "value_form": "0.6",
                  "value_season": "9.8",
                  "web_name": "Zinchenko",
                  "minutes": 811,
                  "goals_scored": 0,
                  "assists": 1,
                  "clean_sheets": 6,
                  "goals_conceded": 5,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 1,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 5,
                  "bps": 217,
                  "influence": "143.8",
                  "creativity": "118.3",
                  "threat": "84.0",
                  "ict_index": "34.6",
                  "starts": 10,
                  "expected_goals": "0.55510",
                  "expected_assists": "0.88711",
                  "expected_goal_involvements": "1.44221",
                  "expected_goals_conceded": "8.53440",
                  "influence_rank": 239,
                  "influence_rank_type": 93,
                  "creativity_rank": 165,
                  "creativity_rank_type": 37,
                  "threat_rank": 212,
                  "threat_rank_type": 44,
                  "ict_index_rank": 237,
                  "ict_index_rank_type": 74,
                  "corners_and_indirect_freekicks_order": null,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": null,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.0616,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.09845,
                  "expected_goal_involvements_per_90": 0.16005,
                  "expected_goals_conceded_per_90": 0.9471,
                  "goals_conceded_per_90": 0.55,
                  "now_cost_rank": 207,
                  "now_cost_rank_type": 28,
                  "form_rank": 89,
                  "form_rank_type": 29,
                  "points_per_game_rank": 38,
                  "points_per_game_rank_type": 11,
                  "selected_rank": 47,
                  "selected_rank_type": 17,
                  "starts_per_90": 1.10974,
                  "clean_sheets_per_90": 0.66584
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(1, teams.Length);

        var elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(2, elements.Length);
        Assert.AreEqual(teams[0].Id, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(1, elements[0].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Midfielder, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);

        Assert.AreEqual(teams[0].Id, elements[1].TeamId);
        Assert.AreEqual(313, elements[1].ElementId);
        Assert.AreEqual(1, elements[1].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Defender, elements[1].ElementType);
        Assert.AreEqual("Oleksandr", elements[1].FirstName);
        Assert.AreEqual("Zinchenko", elements[1].SecondName);
        Assert.AreEqual("Zinchenko", elements[1].WebName);

        json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                }
            ],
            "elements": [
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 75,
                  "code": 223340,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 1,
                  "cost_change_start_fall": -1,
                  "dreamteam_count": 2,
                  "element_type": 3,
                  "ep_next": "6.5",
                  "ep_this": "4.5",
                  "event_points": 6,
                  "first_name": "Bukayo",
                  "form": "6.0",
                  "id": 13,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2023-01-13T15:00:07.612275Z",
                  "now_cost": 81,
                  "photo": "223340.jpg",
                  "points_per_game": "5.6",
                  "second_name": "Saka",
                  "selected_by_percent": "20.5",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 100,
                  "transfers_in": 2864171,
                  "transfers_in_event": 76897,
                  "transfers_out": 2891608,
                  "transfers_out_event": 55853,
                  "value_form": "0.7",
                  "value_season": "12.3",
                  "web_name": "Saka",
                  "minutes": 1534,
                  "goals_scored": 6,
                  "assists": 8,
                  "clean_sheets": 8,
                  "goals_conceded": 14,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 4,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 7,
                  "bps": 339,
                  "influence": "468.8",
                  "creativity": "567.8",
                  "threat": "517.0",
                  "ict_index": "155.4",
                  "starts": 18,
                  "expected_goals": "5.80730",
                  "expected_assists": "2.62779",
                  "expected_goal_involvements": "8.43509",
                  "expected_goals_conceded": "15.99530",
                  "influence_rank": 14,
                  "influence_rank_type": 3,
                  "creativity_rank": 7,
                  "creativity_rank_type": 5,
                  "threat_rank": 22,
                  "threat_rank_type": 13,
                  "ict_index_rank": 10,
                  "ict_index_rank_type": 6,
                  "corners_and_indirect_freekicks_order": 2,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": 1,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.34072,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.15417,
                  "expected_goal_involvements_per_90": 0.49489,
                  "expected_goals_conceded_per_90": 0.93845,
                  "goals_conceded_per_90": 0.82,
                  "now_cost_rank": 15,
                  "now_cost_rank_type": 7,
                  "form_rank": 15,
                  "form_rank_type": 7,
                  "points_per_game_rank": 14,
                  "points_per_game_rank_type": 6,
                  "selected_rank": 19,
                  "selected_rank_type": 8,
                  "starts_per_90": 1.05606,
                  "clean_sheets_per_90": 0.46936
                },
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 100,
                  "code": 206325,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 0,
                  "cost_change_start_fall": 0,
                  "dreamteam_count": 1,
                  "element_type": 2,
                  "ep_next": "3.7",
                  "ep_this": "3.2",
                  "event_points": 6,
                  "first_name": "Oleksandr",
                  "form": "3.2",
                  "id": 313,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2022-12-20T01:00:07.388064Z",
                  "now_cost": 50,
                  "photo": "206325.jpg",
                  "points_per_game": "4.5",
                  "second_name": "Zinchenko",
                  "selected_by_percent": "7.3",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 49,
                  "transfers_in": 1865110,
                  "transfers_in_event": 14505,
                  "transfers_out": 3077704,
                  "transfers_out_event": 5053,
                  "value_form": "0.6",
                  "value_season": "9.8",
                  "web_name": "Zinchenko",
                  "minutes": 811,
                  "goals_scored": 0,
                  "assists": 1,
                  "clean_sheets": 6,
                  "goals_conceded": 5,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 1,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 5,
                  "bps": 217,
                  "influence": "143.8",
                  "creativity": "118.3",
                  "threat": "84.0",
                  "ict_index": "34.6",
                  "starts": 10,
                  "expected_goals": "0.55510",
                  "expected_assists": "0.88711",
                  "expected_goal_involvements": "1.44221",
                  "expected_goals_conceded": "8.53440",
                  "influence_rank": 239,
                  "influence_rank_type": 93,
                  "creativity_rank": 165,
                  "creativity_rank_type": 37,
                  "threat_rank": 212,
                  "threat_rank_type": 44,
                  "ict_index_rank": 237,
                  "ict_index_rank_type": 74,
                  "corners_and_indirect_freekicks_order": null,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": null,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.0616,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.09845,
                  "expected_goal_involvements_per_90": 0.16005,
                  "expected_goals_conceded_per_90": 0.9471,
                  "goals_conceded_per_90": 0.55,
                  "now_cost_rank": 207,
                  "now_cost_rank_type": 28,
                  "form_rank": 89,
                  "form_rank_type": 29,
                  "points_per_game_rank": 38,
                  "points_per_game_rank_type": 11,
                  "selected_rank": 47,
                  "selected_rank_type": 17,
                  "starts_per_90": 1.10974,
                  "clean_sheets_per_90": 0.66584
                },
                {
                  "chance_of_playing_next_round": null,
                  "chance_of_playing_this_round": null,
                  "code": 444145,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 8,
                  "cost_change_start_fall": -8,
                  "dreamteam_count": 2,
                  "element_type": 3,
                  "ep_next": "6.3",
                  "ep_this": "5.8",
                  "event_points": 2,
                  "first_name": "Gabriel",
                  "form": "5.8",
                  "id": 19,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": null,
                  "now_cost": 68,
                  "photo": "444145.jpg",
                  "points_per_game": "5.6",
                  "second_name": "Martinelli Silva",
                  "selected_by_percent": "47.0",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 100,
                  "transfers_in": 5562985,
                  "transfers_in_event": 35286,
                  "transfers_out": 3037275,
                  "transfers_out_event": 114384,
                  "value_form": "0.9",
                  "value_season": "14.7",
                  "web_name": "Martinelli",
                  "minutes": 1528,
                  "goals_scored": 7,
                  "assists": 5,
                  "clean_sheets": 9,
                  "goals_conceded": 14,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 2,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 7,
                  "bps": 315,
                  "influence": "421.2",
                  "creativity": "424.0",
                  "threat": "748.0",
                  "ict_index": "159.3",
                  "starts": 18,
                  "expected_goals": "3.99210",
                  "expected_assists": "3.85744",
                  "expected_goal_involvements": "7.84954",
                  "expected_goals_conceded": "16.13830",
                  "influence_rank": 29,
                  "influence_rank_type": 13,
                  "creativity_rank": 22,
                  "creativity_rank_type": 16,
                  "threat_rank": 8,
                  "threat_rank_type": 3,
                  "ict_index_rank": 5,
                  "ict_index_rank_type": 3,
                  "corners_and_indirect_freekicks_order": 1,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": 2,
                  "direct_freekicks_text": "",
                  "penalties_order": 2,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.23514,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.22721,
                  "expected_goal_involvements_per_90": 0.46234,
                  "expected_goals_conceded_per_90": 0.95055,
                  "goals_conceded_per_90": 0.82,
                  "now_cost_rank": 49,
                  "now_cost_rank_type": 26,
                  "form_rank": 18,
                  "form_rank_type": 10,
                  "points_per_game_rank": 13,
                  "points_per_game_rank_type": 5,
                  "selected_rank": 3,
                  "selected_rank_type": 1,
                  "starts_per_90": 1.06021,
                  "clean_sheets_per_90": 0.5301
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(1, teams.Length);

        elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(3, elements.Length);

        Assert.AreEqual(teams[0].Id, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(1, elements[0].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Midfielder, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);

        Assert.AreEqual(teams[0].Id, elements[1].TeamId);
        Assert.AreEqual(313, elements[1].ElementId);
        Assert.AreEqual(1, elements[1].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Defender, elements[1].ElementType);
        Assert.AreEqual("Oleksandr", elements[1].FirstName);
        Assert.AreEqual("Zinchenko", elements[1].SecondName);
        Assert.AreEqual("Zinchenko", elements[1].WebName);

        Assert.AreEqual(teams[0].Id, elements[2].TeamId);
        Assert.AreEqual(19, elements[2].ElementId);
        Assert.AreEqual(1, elements[2].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Midfielder, elements[2].ElementType);
        Assert.AreEqual("Gabriel", elements[2].FirstName);
        Assert.AreEqual("Martinelli Silva", elements[2].SecondName);
        Assert.AreEqual("Martinelli", elements[2].WebName);
    }

    [TestMethod]
    public async Task ElementSameTest()
    {
        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                }
            ],
            "elements": [
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 75,
                  "code": 223340,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 1,
                  "cost_change_start_fall": -1,
                  "dreamteam_count": 2,
                  "element_type": 3,
                  "ep_next": "6.5",
                  "ep_this": "4.5",
                  "event_points": 6,
                  "first_name": "Bukayo",
                  "form": "6.0",
                  "id": 13,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2023-01-13T15:00:07.612275Z",
                  "now_cost": 81,
                  "photo": "223340.jpg",
                  "points_per_game": "5.6",
                  "second_name": "Saka",
                  "selected_by_percent": "20.5",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 100,
                  "transfers_in": 2864171,
                  "transfers_in_event": 76897,
                  "transfers_out": 2891608,
                  "transfers_out_event": 55853,
                  "value_form": "0.7",
                  "value_season": "12.3",
                  "web_name": "Saka",
                  "minutes": 1534,
                  "goals_scored": 6,
                  "assists": 8,
                  "clean_sheets": 8,
                  "goals_conceded": 14,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 4,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 7,
                  "bps": 339,
                  "influence": "468.8",
                  "creativity": "567.8",
                  "threat": "517.0",
                  "ict_index": "155.4",
                  "starts": 18,
                  "expected_goals": "5.80730",
                  "expected_assists": "2.62779",
                  "expected_goal_involvements": "8.43509",
                  "expected_goals_conceded": "15.99530",
                  "influence_rank": 14,
                  "influence_rank_type": 3,
                  "creativity_rank": 7,
                  "creativity_rank_type": 5,
                  "threat_rank": 22,
                  "threat_rank_type": 13,
                  "ict_index_rank": 10,
                  "ict_index_rank_type": 6,
                  "corners_and_indirect_freekicks_order": 2,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": 1,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.34072,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.15417,
                  "expected_goal_involvements_per_90": 0.49489,
                  "expected_goals_conceded_per_90": 0.93845,
                  "goals_conceded_per_90": 0.82,
                  "now_cost_rank": 15,
                  "now_cost_rank_type": 7,
                  "form_rank": 15,
                  "form_rank_type": 7,
                  "points_per_game_rank": 14,
                  "points_per_game_rank_type": 6,
                  "selected_rank": 19,
                  "selected_rank_type": 8,
                  "starts_per_90": 1.05606,
                  "clean_sheets_per_90": 0.46936
                },
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 100,
                  "code": 206325,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 0,
                  "cost_change_start_fall": 0,
                  "dreamteam_count": 1,
                  "element_type": 2,
                  "ep_next": "3.7",
                  "ep_this": "3.2",
                  "event_points": 6,
                  "first_name": "Oleksandr",
                  "form": "3.2",
                  "id": 313,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2022-12-20T01:00:07.388064Z",
                  "now_cost": 50,
                  "photo": "206325.jpg",
                  "points_per_game": "4.5",
                  "second_name": "Zinchenko",
                  "selected_by_percent": "7.3",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 49,
                  "transfers_in": 1865110,
                  "transfers_in_event": 14505,
                  "transfers_out": 3077704,
                  "transfers_out_event": 5053,
                  "value_form": "0.6",
                  "value_season": "9.8",
                  "web_name": "Zinchenko",
                  "minutes": 811,
                  "goals_scored": 0,
                  "assists": 1,
                  "clean_sheets": 6,
                  "goals_conceded": 5,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 1,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 5,
                  "bps": 217,
                  "influence": "143.8",
                  "creativity": "118.3",
                  "threat": "84.0",
                  "ict_index": "34.6",
                  "starts": 10,
                  "expected_goals": "0.55510",
                  "expected_assists": "0.88711",
                  "expected_goal_involvements": "1.44221",
                  "expected_goals_conceded": "8.53440",
                  "influence_rank": 239,
                  "influence_rank_type": 93,
                  "creativity_rank": 165,
                  "creativity_rank_type": 37,
                  "threat_rank": 212,
                  "threat_rank_type": 44,
                  "ict_index_rank": 237,
                  "ict_index_rank_type": 74,
                  "corners_and_indirect_freekicks_order": null,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": null,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.0616,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.09845,
                  "expected_goal_involvements_per_90": 0.16005,
                  "expected_goals_conceded_per_90": 0.9471,
                  "goals_conceded_per_90": 0.55,
                  "now_cost_rank": 207,
                  "now_cost_rank_type": 28,
                  "form_rank": 89,
                  "form_rank_type": 29,
                  "points_per_game_rank": 38,
                  "points_per_game_rank_type": 11,
                  "selected_rank": 47,
                  "selected_rank_type": 17,
                  "starts_per_90": 1.10974,
                  "clean_sheets_per_90": 0.66584
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(1, teams.Length);

        var elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(2, elements.Length);
        Assert.AreEqual(teams[0].Id, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(1, elements[0].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Midfielder, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);

        Assert.AreEqual(teams[0].Id, elements[1].TeamId);
        Assert.AreEqual(313, elements[1].ElementId);
        Assert.AreEqual(1, elements[1].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Defender, elements[1].ElementType);
        Assert.AreEqual("Oleksandr", elements[1].FirstName);
        Assert.AreEqual("Zinchenko", elements[1].SecondName);
        Assert.AreEqual("Zinchenko", elements[1].WebName);

        // call again
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(2, elements.Length);
        Assert.AreEqual(teams[0].Id, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(1, elements[0].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Midfielder, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);

        Assert.AreEqual(teams[0].Id, elements[1].TeamId);
        Assert.AreEqual(313, elements[1].ElementId);
        Assert.AreEqual(1, elements[1].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Defender, elements[1].ElementType);
        Assert.AreEqual("Oleksandr", elements[1].FirstName);
        Assert.AreEqual("Zinchenko", elements[1].SecondName);
        Assert.AreEqual("Zinchenko", elements[1].WebName);
    }


    [TestMethod]
    public async Task ElementDeleteTest()
    {
        var json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                }
            ],
            "elements": [
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 75,
                  "code": 223340,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 1,
                  "cost_change_start_fall": -1,
                  "dreamteam_count": 2,
                  "element_type": 3,
                  "ep_next": "6.5",
                  "ep_this": "4.5",
                  "event_points": 6,
                  "first_name": "Bukayo",
                  "form": "6.0",
                  "id": 13,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2023-01-13T15:00:07.612275Z",
                  "now_cost": 81,
                  "photo": "223340.jpg",
                  "points_per_game": "5.6",
                  "second_name": "Saka",
                  "selected_by_percent": "20.5",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 100,
                  "transfers_in": 2864171,
                  "transfers_in_event": 76897,
                  "transfers_out": 2891608,
                  "transfers_out_event": 55853,
                  "value_form": "0.7",
                  "value_season": "12.3",
                  "web_name": "Saka",
                  "minutes": 1534,
                  "goals_scored": 6,
                  "assists": 8,
                  "clean_sheets": 8,
                  "goals_conceded": 14,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 4,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 7,
                  "bps": 339,
                  "influence": "468.8",
                  "creativity": "567.8",
                  "threat": "517.0",
                  "ict_index": "155.4",
                  "starts": 18,
                  "expected_goals": "5.80730",
                  "expected_assists": "2.62779",
                  "expected_goal_involvements": "8.43509",
                  "expected_goals_conceded": "15.99530",
                  "influence_rank": 14,
                  "influence_rank_type": 3,
                  "creativity_rank": 7,
                  "creativity_rank_type": 5,
                  "threat_rank": 22,
                  "threat_rank_type": 13,
                  "ict_index_rank": 10,
                  "ict_index_rank_type": 6,
                  "corners_and_indirect_freekicks_order": 2,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": 1,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.34072,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.15417,
                  "expected_goal_involvements_per_90": 0.49489,
                  "expected_goals_conceded_per_90": 0.93845,
                  "goals_conceded_per_90": 0.82,
                  "now_cost_rank": 15,
                  "now_cost_rank_type": 7,
                  "form_rank": 15,
                  "form_rank_type": 7,
                  "points_per_game_rank": 14,
                  "points_per_game_rank_type": 6,
                  "selected_rank": 19,
                  "selected_rank_type": 8,
                  "starts_per_90": 1.05606,
                  "clean_sheets_per_90": 0.46936
                },
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 100,
                  "code": 206325,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 0,
                  "cost_change_start_fall": 0,
                  "dreamteam_count": 1,
                  "element_type": 2,
                  "ep_next": "3.7",
                  "ep_this": "3.2",
                  "event_points": 6,
                  "first_name": "Oleksandr",
                  "form": "3.2",
                  "id": 313,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2022-12-20T01:00:07.388064Z",
                  "now_cost": 50,
                  "photo": "206325.jpg",
                  "points_per_game": "4.5",
                  "second_name": "Zinchenko",
                  "selected_by_percent": "7.3",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 49,
                  "transfers_in": 1865110,
                  "transfers_in_event": 14505,
                  "transfers_out": 3077704,
                  "transfers_out_event": 5053,
                  "value_form": "0.6",
                  "value_season": "9.8",
                  "web_name": "Zinchenko",
                  "minutes": 811,
                  "goals_scored": 0,
                  "assists": 1,
                  "clean_sheets": 6,
                  "goals_conceded": 5,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 1,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 5,
                  "bps": 217,
                  "influence": "143.8",
                  "creativity": "118.3",
                  "threat": "84.0",
                  "ict_index": "34.6",
                  "starts": 10,
                  "expected_goals": "0.55510",
                  "expected_assists": "0.88711",
                  "expected_goal_involvements": "1.44221",
                  "expected_goals_conceded": "8.53440",
                  "influence_rank": 239,
                  "influence_rank_type": 93,
                  "creativity_rank": 165,
                  "creativity_rank_type": 37,
                  "threat_rank": 212,
                  "threat_rank_type": 44,
                  "ict_index_rank": 237,
                  "ict_index_rank_type": 74,
                  "corners_and_indirect_freekicks_order": null,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": null,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.0616,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.09845,
                  "expected_goal_involvements_per_90": 0.16005,
                  "expected_goals_conceded_per_90": 0.9471,
                  "goals_conceded_per_90": 0.55,
                  "now_cost_rank": 207,
                  "now_cost_rank_type": 28,
                  "form_rank": 89,
                  "form_rank_type": 29,
                  "points_per_game_rank": 38,
                  "points_per_game_rank_type": 11,
                  "selected_rank": 47,
                  "selected_rank_type": 17,
                  "starts_per_90": 1.10974,
                  "clean_sheets_per_90": 0.66584
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(1, teams.Length);

        var elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(2, elements.Length);
        Assert.AreEqual(1, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(teams[0].Id, elements[0].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Midfielder, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);

        Assert.AreEqual(1, elements[1].TeamId);
        Assert.AreEqual(313, elements[1].ElementId);
        Assert.AreEqual(teams[0].Id, elements[1].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Defender, elements[1].ElementType);
        Assert.AreEqual("Oleksandr", elements[1].FirstName);
        Assert.AreEqual("Zinchenko", elements[1].SecondName);
        Assert.AreEqual("Zinchenko", elements[1].WebName);

        json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": true
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                }
            ],
            "elements": [
                        {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 100,
                  "code": 206325,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 0,
                  "cost_change_start_fall": 0,
                  "dreamteam_count": 1,
                  "element_type": 2,
                  "ep_next": "3.7",
                  "ep_this": "3.2",
                  "event_points": 6,
                  "first_name": "Oleksandr",
                  "form": "3.2",
                  "id": 313,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2022-12-20T01:00:07.388064Z",
                  "now_cost": 50,
                  "photo": "206325.jpg",
                  "points_per_game": "4.5",
                  "second_name": "Zinchenko",
                  "selected_by_percent": "7.3",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 49,
                  "transfers_in": 1865110,
                  "transfers_in_event": 14505,
                  "transfers_out": 3077704,
                  "transfers_out_event": 5053,
                  "value_form": "0.6",
                  "value_season": "9.8",
                  "web_name": "Zinchenko",
                  "minutes": 811,
                  "goals_scored": 0,
                  "assists": 1,
                  "clean_sheets": 6,
                  "goals_conceded": 5,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 1,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 5,
                  "bps": 217,
                  "influence": "143.8",
                  "creativity": "118.3",
                  "threat": "84.0",
                  "ict_index": "34.6",
                  "starts": 10,
                  "expected_goals": "0.55510",
                  "expected_assists": "0.88711",
                  "expected_goal_involvements": "1.44221",
                  "expected_goals_conceded": "8.53440",
                  "influence_rank": 239,
                  "influence_rank_type": 93,
                  "creativity_rank": 165,
                  "creativity_rank_type": 37,
                  "threat_rank": 212,
                  "threat_rank_type": 44,
                  "ict_index_rank": 237,
                  "ict_index_rank_type": 74,
                  "corners_and_indirect_freekicks_order": null,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": null,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.0616,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.09845,
                  "expected_goal_involvements_per_90": 0.16005,
                  "expected_goals_conceded_per_90": 0.9471,
                  "goals_conceded_per_90": 0.55,
                  "now_cost_rank": 207,
                  "now_cost_rank_type": 28,
                  "form_rank": 89,
                  "form_rank_type": 29,
                  "points_per_game_rank": 38,
                  "points_per_game_rank_type": 11,
                  "selected_rank": 47,
                  "selected_rank_type": 17,
                  "starts_per_90": 1.10974,
                  "clean_sheets_per_90": 0.66584
                }
            ]
        }
        """;
        await ExecuteApi<BootstrapStaticApi>(new Models.MockHttpParameter() { ResponseContent = json });

        teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(1, teams.Length);

        elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(1, elements[0].TeamId);
        Assert.AreEqual(313, elements[0].ElementId);
        Assert.AreEqual(teams[0].Id, elements[0].ElementTeamId);
        Assert.AreEqual(Database.Models.ElementType.Defender, elements[0].ElementType);
        Assert.AreEqual("Oleksandr", elements[0].FirstName);
        Assert.AreEqual("Zinchenko", elements[0].SecondName);
        Assert.AreEqual("Zinchenko", elements[0].WebName);
    }

    /// <summary>
    /// Assert the Database model and the API model for the Team is correct
    /// </summary>
    /// <param name="dbTeam">Team model stored in Database</param>
    /// <param name="team">Team model returned from API</param>
    /// <param name="seasonId">Id of the season</param>
    private static void AssertTeam(Database.Models.Team dbTeam, Team team, int seasonId)
    {
        Assert.AreEqual(team.Id, dbTeam.TeamId);
        Assert.AreEqual(team.Name, dbTeam.Name);
        Assert.AreEqual(team.ShortName, dbTeam.ShortName);
        Assert.AreEqual(team.Code, dbTeam.Code);
        Assert.AreEqual(seasonId, dbTeam.SeasonId);
    }

    /// <summary>
    /// Assert the Element model and the API model for the Element is correct
    /// </summary>
    /// <param name="dbElement">Element model stored in Database</param>
    /// <param name="element">Element model returned from API</param>
    /// <param name="teamId">Id of the team</param>
    private static void AssertElement(Database.Models.Element dbElement, Element element, int teamId)
    {
        Assert.AreEqual(element.FirstName, dbElement.FirstName);
        Assert.AreEqual(element.SecondName, dbElement.SecondName);
        Assert.AreEqual(element.WebName, dbElement.WebName);
        Assert.AreEqual(element.Code, dbElement.Code);
        Assert.AreEqual(element.ElementType, (int)dbElement.ElementType);
        Assert.AreEqual(teamId, dbElement.TeamId);
        Assert.AreEqual(element.Team, dbElement.ElementTeamId);
    }
}