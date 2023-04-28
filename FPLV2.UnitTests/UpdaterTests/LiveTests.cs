using FPLV2.Client.Models;
using FPLV2.Updater.Api;
using System.Text.Json;

namespace FPLV2.UnitTests.UpdaterTests;

/// <summary>
/// Tests the corresponding HistoryApi class
/// </summary>
[TestClass]
public class LiveTests : UpdaterTests
{
    /// <summary>
    /// Gets the request url to be matched against
    /// </summary>
    protected override string RequestUrl => "event/{0}/live/";

    /// <summary>
    /// Gets the Sample Data json file name
    /// </summary>
    protected override string ResourceName => "live";

    [TestMethod]
    public async Task RealDataTest()
    {
        // insert season used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert fake elements, up to 1000 which should cover all ids in the json
        for (var i = 0; i < 1000; i++)
        {
            await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = (i + 1), TeamId = teamId });
        }

        var json = GetLiveDataJson();
        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });

        var model = JsonSerializer.Deserialize<ElementStatRoot>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);
        var elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        // grab a few random element stats to check
        var rand = new Random();

        for (var i = 0; i < 10; i++)
        {
            var elementStat = model.Elements[rand.Next(model.Elements.Length)];
            var dbElementStat = elementStats.FirstOrDefault(x => x.ApiElementId == elementStat.Id && x.Gameweek == BaseApi.CurrentGameweek);
            Assert.IsNotNull(dbElementStat);
            AssertElementStat(dbElementStat, elementStat, elements);
        }
    }

    [TestMethod]
    public async Task NoSeasonIdSetTest()
    {
        var json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            }]
        }
        """;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = 0;
        BaseApi.CurrentGameweek = 1; // have to set it to 0 because other tests will set this

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 113, TeamId = teamId });

        var success = await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });
        Assert.IsFalse(success);
    }

    [TestMethod]
    public async Task NoCurrentGameweekSetTest()
    {
        var json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            }]
        }
        """;

        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 0; // have to set it to 0 because other tests will set this

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });

        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 113, TeamId = teamId });

        var success = await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });
        Assert.IsFalse(success);
    }

    [TestMethod]
    public async Task ElementStatNewTest()
    {
        var json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            }]
        }
        """;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });


        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 28, TeamId = teamId });
        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });

        var elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(0, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("3.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(0, elementStats[0].GoalsScored);
        Assert.AreEqual("4.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("7.4", elementStats[0].Influence);
        Assert.AreEqual(82, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("38.0", elementStats[0].Threat);
        Assert.AreEqual(2, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);

        json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            },
            {
          "id": 318,
          "stats": {
            "minutes": 77,
            "goals_scored": 2,
            "assists": 0,
            "clean_sheets": 1,
            "goals_conceded": 0,
            "own_goals": 0,
            "penalties_saved": 0,
            "penalties_missed": 0,
            "yellow_cards": 0,
            "red_cards": 0,
            "saves": 0,
            "bonus": 3,
            "bps": 48,
            "influence": "62.6",
            "creativity": "4.8",
            "threat": "73.0",
            "ict_index": "14.0",
            "starts": 1,
            "expected_goals": "1.66",
            "expected_assists": "0.08",
            "expected_goal_involvements": "1.74",
            "expected_goals_conceded": "0.49",
            "total_points": 13,
            "in_dreamteam": true
          }
        }]
        }
        """;

        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 318, TeamId = teamId });
        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });
        elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(2, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(0, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("3.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(0, elementStats[0].GoalsScored);
        Assert.AreEqual("4.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("7.4", elementStats[0].Influence);
        Assert.AreEqual(82, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("38.0", elementStats[0].Threat);
        Assert.AreEqual(2, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);

        Assert.AreEqual(318, elementStats[1].ApiElementId);
        Assert.AreEqual(0, elementStats[1].Assists);
        Assert.AreEqual(3, elementStats[1].Bonus);
        Assert.AreEqual(48, elementStats[1].Bps);
        Assert.AreEqual(1, elementStats[1].CleanSheets);
        Assert.AreEqual("4.8", elementStats[1].Creativity);
        Assert.AreEqual(0, elementStats[1].GoalsConceded);
        Assert.AreEqual(2, elementStats[1].GoalsScored);
        Assert.AreEqual("14.0", elementStats[1].IctIndex);
        Assert.AreEqual(true, elementStats[1].InDreamteam);
        Assert.AreEqual("62.6", elementStats[1].Influence);
        Assert.AreEqual(77, elementStats[1].Minutes);
        Assert.AreEqual(0, elementStats[1].OwnGoals);
        Assert.AreEqual(0, elementStats[1].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[1].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[1].RedCards);
        Assert.AreEqual(0, elementStats[1].Saves);
        Assert.AreEqual("73.0", elementStats[1].Threat);
        Assert.AreEqual(13, elementStats[1].TotalPoints);
        Assert.AreEqual(0, elementStats[1].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[1].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 318)?.Id ?? 0, elementStats[1].ElementId);
    }


    [TestMethod]
    public async Task ElementStatUpdateTest()
    {
        var json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            }]
        }
        """;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });


        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 28, TeamId = teamId });
        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });

        var elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(0, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("3.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(0, elementStats[0].GoalsScored);
        Assert.AreEqual("4.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("7.4", elementStats[0].Influence);
        Assert.AreEqual(82, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("38.0", elementStats[0].Threat);
        Assert.AreEqual(2, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);

        json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 90,
                "goals_scored": 6,
                "assists": 5,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "17.4",
                "creativity": "13.2",
                "threat": "138.0",
                "ict_index": "14.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 22,
                "in_dreamteam": false
              }     
            }]
        }
        """;

        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 318, TeamId = teamId });
        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });
        elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(5, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("13.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(6, elementStats[0].GoalsScored);
        Assert.AreEqual("14.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("17.4", elementStats[0].Influence);
        Assert.AreEqual(90, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("138.0", elementStats[0].Threat);
        Assert.AreEqual(22, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);
    }


    [TestMethod]
    public async Task ElementStatDeleteTest()
    {
        var json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            },
            {
          "id": 318,
          "stats": {
            "minutes": 77,
            "goals_scored": 2,
            "assists": 0,
            "clean_sheets": 1,
            "goals_conceded": 0,
            "own_goals": 0,
            "penalties_saved": 0,
            "penalties_missed": 0,
            "yellow_cards": 0,
            "red_cards": 0,
            "saves": 0,
            "bonus": 3,
            "bps": 48,
            "influence": "62.6",
            "creativity": "4.8",
            "threat": "73.0",
            "ict_index": "14.0",
            "starts": 1,
            "expected_goals": "1.66",
            "expected_assists": "0.08",
            "expected_goal_involvements": "1.74",
            "expected_goals_conceded": "0.49",
            "total_points": 13,
            "in_dreamteam": true
          }
        }]
        }
        """;

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });


        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 318, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 28, TeamId = teamId });
        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });

        var elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(2, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(0, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("3.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(0, elementStats[0].GoalsScored);
        Assert.AreEqual("4.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("7.4", elementStats[0].Influence);
        Assert.AreEqual(82, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("38.0", elementStats[0].Threat);
        Assert.AreEqual(2, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);

        Assert.AreEqual(318, elementStats[1].ApiElementId);
        Assert.AreEqual(0, elementStats[1].Assists);
        Assert.AreEqual(3, elementStats[1].Bonus);
        Assert.AreEqual(48, elementStats[1].Bps);
        Assert.AreEqual(1, elementStats[1].CleanSheets);
        Assert.AreEqual("4.8", elementStats[1].Creativity);
        Assert.AreEqual(0, elementStats[1].GoalsConceded);
        Assert.AreEqual(2, elementStats[1].GoalsScored);
        Assert.AreEqual("14.0", elementStats[1].IctIndex);
        Assert.AreEqual(true, elementStats[1].InDreamteam);
        Assert.AreEqual("62.6", elementStats[1].Influence);
        Assert.AreEqual(77, elementStats[1].Minutes);
        Assert.AreEqual(0, elementStats[1].OwnGoals);
        Assert.AreEqual(0, elementStats[1].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[1].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[1].RedCards);
        Assert.AreEqual(0, elementStats[1].Saves);
        Assert.AreEqual("73.0", elementStats[1].Threat);
        Assert.AreEqual(13, elementStats[1].TotalPoints);
        Assert.AreEqual(0, elementStats[1].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[1].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 318)?.Id ?? 0, elementStats[1].ElementId);

        json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            }]
        }
        """;

        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });
        elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(0, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("3.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(0, elementStats[0].GoalsScored);
        Assert.AreEqual("4.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("7.4", elementStats[0].Influence);
        Assert.AreEqual(82, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("38.0", elementStats[0].Threat);
        Assert.AreEqual(2, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);
    }

    [TestMethod]
    public async Task ElementStatNewHasPreviousTest()
    {
        var json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            }]
        }
        """;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldTeamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = oldSeasonId });
        var oldElementId = await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 1, TeamId = oldTeamId });
        await UnitOfWork.ElementStats.Insert(new Database.Models.ElementStat() { ElementId = oldElementId, Gameweek = 1, GoalsScored = 6, Assists = 5 });

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });


        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 28, TeamId = teamId });
        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });

        var elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(0, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("3.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(0, elementStats[0].GoalsScored);
        Assert.AreEqual("4.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("7.4", elementStats[0].Influence);
        Assert.AreEqual(82, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("38.0", elementStats[0].Threat);
        Assert.AreEqual(2, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);

        json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            },
            {
          "id": 318,
          "stats": {
            "minutes": 77,
            "goals_scored": 2,
            "assists": 0,
            "clean_sheets": 1,
            "goals_conceded": 0,
            "own_goals": 0,
            "penalties_saved": 0,
            "penalties_missed": 0,
            "yellow_cards": 0,
            "red_cards": 0,
            "saves": 0,
            "bonus": 3,
            "bps": 48,
            "influence": "62.6",
            "creativity": "4.8",
            "threat": "73.0",
            "ict_index": "14.0",
            "starts": 1,
            "expected_goals": "1.66",
            "expected_assists": "0.08",
            "expected_goal_involvements": "1.74",
            "expected_goals_conceded": "0.49",
            "total_points": 13,
            "in_dreamteam": true
          }
        }]
        }
        """;

        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 318, TeamId = teamId });
        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });
        elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(2, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(0, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("3.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(0, elementStats[0].GoalsScored);
        Assert.AreEqual("4.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("7.4", elementStats[0].Influence);
        Assert.AreEqual(82, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("38.0", elementStats[0].Threat);
        Assert.AreEqual(2, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);

        Assert.AreEqual(318, elementStats[1].ApiElementId);
        Assert.AreEqual(0, elementStats[1].Assists);
        Assert.AreEqual(3, elementStats[1].Bonus);
        Assert.AreEqual(48, elementStats[1].Bps);
        Assert.AreEqual(1, elementStats[1].CleanSheets);
        Assert.AreEqual("4.8", elementStats[1].Creativity);
        Assert.AreEqual(0, elementStats[1].GoalsConceded);
        Assert.AreEqual(2, elementStats[1].GoalsScored);
        Assert.AreEqual("14.0", elementStats[1].IctIndex);
        Assert.AreEqual(true, elementStats[1].InDreamteam);
        Assert.AreEqual("62.6", elementStats[1].Influence);
        Assert.AreEqual(77, elementStats[1].Minutes);
        Assert.AreEqual(0, elementStats[1].OwnGoals);
        Assert.AreEqual(0, elementStats[1].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[1].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[1].RedCards);
        Assert.AreEqual(0, elementStats[1].Saves);
        Assert.AreEqual("73.0", elementStats[1].Threat);
        Assert.AreEqual(13, elementStats[1].TotalPoints);
        Assert.AreEqual(0, elementStats[1].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[1].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 318)?.Id ?? 0, elementStats[1].ElementId);

        // make sure old data is fine
        elements = await UnitOfWork.Elements.GetAllBySeasonId(oldSeasonId);
        elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(oldSeasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(6, elementStats[0].GoalsScored);
        Assert.AreEqual(5, elementStats[0].Assists);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 1)?.Id ?? 0, elementStats[0].ElementId);
    }

    [TestMethod]
    public async Task ElementStatUpdateHasPreviousTest()
    {
        var json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            }]
        }
        """;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldTeamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = oldSeasonId });
        var oldElementId = await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 1, TeamId = oldTeamId });
        await UnitOfWork.ElementStats.Insert(new Database.Models.ElementStat() { ElementId = oldElementId, Gameweek = 1, GoalsScored = 6, Assists = 5 });

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });


        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 28, TeamId = teamId });
        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });

        var elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(0, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("3.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(0, elementStats[0].GoalsScored);
        Assert.AreEqual("4.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("7.4", elementStats[0].Influence);
        Assert.AreEqual(82, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("38.0", elementStats[0].Threat);
        Assert.AreEqual(2, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);

        json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 90,
                "goals_scored": 6,
                "assists": 5,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "17.4",
                "creativity": "13.2",
                "threat": "138.0",
                "ict_index": "14.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 22,
                "in_dreamteam": false
              }     
            }]
        }
        """;

        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 318, TeamId = teamId });
        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });
        elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(5, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("13.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(6, elementStats[0].GoalsScored);
        Assert.AreEqual("14.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("17.4", elementStats[0].Influence);
        Assert.AreEqual(90, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("138.0", elementStats[0].Threat);
        Assert.AreEqual(22, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);

        // make sure old data is fine
        elements = await UnitOfWork.Elements.GetAllBySeasonId(oldSeasonId);
        elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(oldSeasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(6, elementStats[0].GoalsScored);
        Assert.AreEqual(5, elementStats[0].Assists);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 1)?.Id ?? 0, elementStats[0].ElementId);
    }


    [TestMethod]
    public async Task ElementStatDeleteHasPreviousTest()
    {
        var json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            },
            {
          "id": 318,
          "stats": {
            "minutes": 77,
            "goals_scored": 2,
            "assists": 0,
            "clean_sheets": 1,
            "goals_conceded": 0,
            "own_goals": 0,
            "penalties_saved": 0,
            "penalties_missed": 0,
            "yellow_cards": 0,
            "red_cards": 0,
            "saves": 0,
            "bonus": 3,
            "bps": 48,
            "influence": "62.6",
            "creativity": "4.8",
            "threat": "73.0",
            "ict_index": "14.0",
            "starts": 1,
            "expected_goals": "1.66",
            "expected_assists": "0.08",
            "expected_goal_involvements": "1.74",
            "expected_goals_conceded": "0.49",
            "total_points": 13,
            "in_dreamteam": true
          }
        }]
        }
        """;

        // add some data for previous year, use the same team ids and names as our test ones so we make sure these don't get deleted
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2000/01" });
        var oldTeamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = oldSeasonId });
        var oldElementId = await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 1, TeamId = oldTeamId });
        await UnitOfWork.ElementStats.Insert(new Database.Models.ElementStat() { ElementId = oldElementId, Gameweek = 1, GoalsScored = 6, Assists = 5 });

        // insert league used in real sample data
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        BaseApi.SeasonId = seasonId;
        BaseApi.CurrentGameweek = 1;

        // insert a fake team to reference for teamid
        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });


        // insert the elements that are being referenced
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 318, TeamId = teamId });
        await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 28, TeamId = teamId });
        var elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });

        var elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(2, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(0, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("3.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(0, elementStats[0].GoalsScored);
        Assert.AreEqual("4.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("7.4", elementStats[0].Influence);
        Assert.AreEqual(82, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("38.0", elementStats[0].Threat);
        Assert.AreEqual(2, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);

        Assert.AreEqual(318, elementStats[1].ApiElementId);
        Assert.AreEqual(0, elementStats[1].Assists);
        Assert.AreEqual(3, elementStats[1].Bonus);
        Assert.AreEqual(48, elementStats[1].Bps);
        Assert.AreEqual(1, elementStats[1].CleanSheets);
        Assert.AreEqual("4.8", elementStats[1].Creativity);
        Assert.AreEqual(0, elementStats[1].GoalsConceded);
        Assert.AreEqual(2, elementStats[1].GoalsScored);
        Assert.AreEqual("14.0", elementStats[1].IctIndex);
        Assert.AreEqual(true, elementStats[1].InDreamteam);
        Assert.AreEqual("62.6", elementStats[1].Influence);
        Assert.AreEqual(77, elementStats[1].Minutes);
        Assert.AreEqual(0, elementStats[1].OwnGoals);
        Assert.AreEqual(0, elementStats[1].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[1].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[1].RedCards);
        Assert.AreEqual(0, elementStats[1].Saves);
        Assert.AreEqual("73.0", elementStats[1].Threat);
        Assert.AreEqual(13, elementStats[1].TotalPoints);
        Assert.AreEqual(0, elementStats[1].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[1].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 318)?.Id ?? 0, elementStats[1].ElementId);

        json = $$"""
        {
            "elements": [
            {
              "id": 28,
              "stats": {
                "minutes": 82,
                "goals_scored": 0,
                "assists": 0,
                "clean_sheets": 1,
                "goals_conceded": 0,
                "own_goals": 0,
                "penalties_saved": 0,
                "penalties_missed": 0,
                "yellow_cards": 0,
                "red_cards": 0,
                "saves": 0,
                "bonus": 0,
                "bps": 7,
                "influence": "7.4",
                "creativity": "3.2",
                "threat": "38.0",
                "ict_index": "4.9",
                "starts": 1,
                "expected_goals": "0.15",
                "expected_assists": "0.08",
                "expected_goal_involvements": "0.23",
                "expected_goals_conceded": "1.21",
                "total_points": 2,
                "in_dreamteam": false
              }     
            }]
        }
        """;

        elements = await UnitOfWork.Elements.GetAllBySeasonId(seasonId);

        await ExecuteApi<LiveApi>(new Models.MockHttpParameter() { RequestUrl = string.Format(RequestUrl, 1), ResponseContent = json });
        elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(seasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(28, elementStats[0].ApiElementId);
        Assert.AreEqual(0, elementStats[0].Assists);
        Assert.AreEqual(0, elementStats[0].Bonus);
        Assert.AreEqual(7, elementStats[0].Bps);
        Assert.AreEqual(1, elementStats[0].CleanSheets);
        Assert.AreEqual("3.2", elementStats[0].Creativity);
        Assert.AreEqual(0, elementStats[0].GoalsConceded);
        Assert.AreEqual(0, elementStats[0].GoalsScored);
        Assert.AreEqual("4.9", elementStats[0].IctIndex);
        Assert.AreEqual(false, elementStats[0].InDreamteam);
        Assert.AreEqual("7.4", elementStats[0].Influence);
        Assert.AreEqual(82, elementStats[0].Minutes);
        Assert.AreEqual(0, elementStats[0].OwnGoals);
        Assert.AreEqual(0, elementStats[0].PenaltiesMissed);
        Assert.AreEqual(0, elementStats[0].PenaltiesSaved);
        Assert.AreEqual(0, elementStats[0].RedCards);
        Assert.AreEqual(0, elementStats[0].Saves);
        Assert.AreEqual("38.0", elementStats[0].Threat);
        Assert.AreEqual(2, elementStats[0].TotalPoints);
        Assert.AreEqual(0, elementStats[0].YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, elementStats[0].Gameweek);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 28)?.Id ?? 0, elementStats[0].ElementId);

        // make sure old data is fine
        elements = await UnitOfWork.Elements.GetAllBySeasonId(oldSeasonId);
        elementStats = await UnitOfWork.ElementStats.GetAllBySeasonId(oldSeasonId);
        Assert.AreEqual(1, elementStats.Length);
        Assert.AreEqual(6, elementStats[0].GoalsScored);
        Assert.AreEqual(5, elementStats[0].Assists);
        Assert.AreEqual(elements.FirstOrDefault(x => x.ElementId == 1)?.Id ?? 0, elementStats[0].ElementId);
    }

    /// <summary>
    /// Assert the Database model and the API model for the ElementStats is correct
    /// </summary>
    /// <param name="dbElementStats">ElementStat model stored in Database</param>
    /// <param name="elementStat">ElementStat model returned from API</param>
    /// <param name="dbElements">Element models stored in Database</param>
    private static void AssertElementStat(Database.Models.ElementStat dbElementStats, ElementStat elementStat, Database.Models.Element[] dbElements)
    {
        Assert.AreEqual(elementStat.Id, dbElementStats.ApiElementId);
        Assert.AreEqual(elementStat.Stats.Assists, dbElementStats.Assists);
        Assert.AreEqual(elementStat.Stats.Bonus, dbElementStats.Bonus);
        Assert.AreEqual(elementStat.Stats.Bps, dbElementStats.Bps);
        Assert.AreEqual(elementStat.Stats.CleanSheets, dbElementStats.CleanSheets);
        Assert.AreEqual(elementStat.Stats.Creativity, dbElementStats.Creativity);
        Assert.AreEqual(elementStat.Stats.GoalsConceded, dbElementStats.GoalsConceded);
        Assert.AreEqual(elementStat.Stats.GoalsScored, dbElementStats.GoalsScored);
        Assert.AreEqual(elementStat.Stats.IctIndex, dbElementStats.IctIndex);
        Assert.AreEqual(elementStat.Stats.InDreamteam, dbElementStats.InDreamteam);
        Assert.AreEqual(elementStat.Stats.Influence, dbElementStats.Influence);
        Assert.AreEqual(elementStat.Stats.Minutes, dbElementStats.Minutes);
        Assert.AreEqual(elementStat.Stats.OwnGoals, dbElementStats.OwnGoals);
        Assert.AreEqual(elementStat.Stats.PenaltiesMissed, dbElementStats.PenaltiesMissed);
        Assert.AreEqual(elementStat.Stats.PenaltiesSaved, dbElementStats.PenaltiesSaved);
        Assert.AreEqual(elementStat.Stats.RedCards, dbElementStats.RedCards);
        Assert.AreEqual(elementStat.Stats.Saves, dbElementStats.Saves);
        Assert.AreEqual(elementStat.Stats.Threat, dbElementStats.Threat);
        Assert.AreEqual(elementStat.Stats.TotalPoints, dbElementStats.TotalPoints);
        Assert.AreEqual(elementStat.Stats.YellowCards, dbElementStats.YellowCards);
        Assert.AreEqual(BaseApi.CurrentGameweek, dbElementStats.Gameweek);
        Assert.AreEqual(dbElements.FirstOrDefault(x => x.ElementId == elementStat.Id)?.Id ?? 0, dbElementStats.ElementId);
    }
}
