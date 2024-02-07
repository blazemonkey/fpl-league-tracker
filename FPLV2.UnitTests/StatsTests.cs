using System.Text.RegularExpressions;

namespace FPLV2.UnitTests;

/// <summary>
/// Tests for the Statistics Stored Procedures
/// </summary>
[TestClass]
public class StatsTests : UnitTests
{
    [TestMethod]
    public async Task StatsMostPointsInAGameweekTest()
    {
        // season 1
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2021/22" });
        var oldLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = 1, SeasonId = oldSeasonId, Name = "PSL" });

        var oldPlayerId1 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = oldLeagueId, EntryId = 1, PlayerName = "Fake Player 1", TeamName = "Fake Team 1" });
        var oldPlayerId2 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = oldLeagueId, EntryId = 2, PlayerName = "Fake Player 2", TeamName = "Fake Team 2" });
        var oldPlayerId3 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = oldLeagueId, EntryId = 3, PlayerName = "Fake Player 3", TeamName = "Fake Team 3" });

        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId1, Gameweek = 1, GameweekPoints = 100, GameweekPointsOnBench = 5, Total = 100 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId1, Gameweek = 2, GameweekPoints = 200, GameweekPointsOnBench = 5, Total = 300 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId2, Gameweek = 1, GameweekPoints = 1, GameweekPointsOnBench = 5, Total = 1 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId2, Gameweek = 2, GameweekPoints = 2, GameweekPointsOnBench = 5, Total = 3 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId3, Gameweek = 1, GameweekPoints = 10, GameweekPointsOnBench = 5, Total = 10 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId3, Gameweek = 2, GameweekPoints = 20, GameweekPointsOnBench = 5, Total = 30 });

        // season 2
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        var leagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = 1, SeasonId = seasonId, Name = "PSL" });

        var playerId1 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = leagueId, EntryId = 1, PlayerName = "Fake Player 1", TeamName = "Fake Team 1" });
        var playerId2 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = leagueId, EntryId = 2, PlayerName = "Fake Player 2", TeamName = "Fake Team 2" });
        var playerId3 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = leagueId, EntryId = 3, PlayerName = "Fake Player 3", TeamName = "Fake Team 3" });

        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId1, Gameweek = 1, GameweekPoints = 5, GameweekPointsOnBench = 5, Total = 5 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId1, Gameweek = 2, GameweekPoints = 6, GameweekPointsOnBench = 5, Total = 11 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId2, Gameweek = 1, GameweekPoints = 50, GameweekPointsOnBench = 5, Total = 50 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId2, Gameweek = 2, GameweekPoints = 60, GameweekPointsOnBench = 5, Total = 110 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId3, Gameweek = 1, GameweekPoints = 500, GameweekPointsOnBench = 5, Total = 500 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId3, Gameweek = 2, GameweekPoints = 600, GameweekPointsOnBench = 5, Total = 1100 });


        var stats = await UnitOfWork.Stats.GetMostPointsInAGameweek(oldSeasonId, 1);
        Assert.AreEqual(6, stats.Count());
        Assert.AreEqual("Fake Team 1", stats[0]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[0]["Gameweek"]);
        Assert.AreEqual(200, (int)stats[0]["Points"]);
        Assert.AreEqual("Fake Team 1", stats[1]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[1]["Gameweek"]);
        Assert.AreEqual(100, (int)stats[1]["Points"]);
        Assert.AreEqual("Fake Team 3", stats[2]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[2]["Gameweek"]);
        Assert.AreEqual(20, (int)stats[2]["Points"]);
        Assert.AreEqual("Fake Team 3", stats[3]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[3]["Gameweek"]);
        Assert.AreEqual(10, (int)stats[3]["Points"]);
        Assert.AreEqual("Fake Team 2", stats[4]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[4]["Gameweek"]);
        Assert.AreEqual(2, (int)stats[4]["Points"]);
        Assert.AreEqual("Fake Team 2", stats[5]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[5]["Gameweek"]);
        Assert.AreEqual(1, (int)stats[5]["Points"]);

        stats = await UnitOfWork.Stats.GetMostPointsInAGameweek(seasonId, 1);
        Assert.AreEqual(6, stats.Count());
        Assert.AreEqual("Fake Team 3", stats[0]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[0]["Gameweek"]);
        Assert.AreEqual(600, (int)stats[0]["Points"]);
        Assert.AreEqual("Fake Team 3", stats[1]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[1]["Gameweek"]);
        Assert.AreEqual(500, (int)stats[1]["Points"]);
        Assert.AreEqual("Fake Team 2", stats[2]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[2]["Gameweek"]);
        Assert.AreEqual(60, (int)stats[2]["Points"]);
        Assert.AreEqual("Fake Team 2", stats[3]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[3]["Gameweek"]);
        Assert.AreEqual(50, (int)stats[3]["Points"]);
        Assert.AreEqual("Fake Team 1", stats[4]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[4]["Gameweek"]);
        Assert.AreEqual(6, (int)stats[4]["Points"]);
        Assert.AreEqual("Fake Team 1", stats[5]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[5]["Gameweek"]);
        Assert.AreEqual(5, (int)stats[5]["Points"]);
    }

    [TestMethod]
    public async Task StatsMostBenchPointsInAGameweekTest()
    {
        // season 1
        var oldSeasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2021/22" });
        var oldLeagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = 1, SeasonId = oldSeasonId, Name = "PSL" });

        var oldPlayerId1 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = oldLeagueId, EntryId = 1, PlayerName = "Fake Player 1", TeamName = "Fake Team 1" });
        var oldPlayerId2 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = oldLeagueId, EntryId = 2, PlayerName = "Fake Player 2", TeamName = "Fake Team 2" });
        var oldPlayerId3 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = oldLeagueId, EntryId = 3, PlayerName = "Fake Player 3", TeamName = "Fake Team 3" });

        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId1, Gameweek = 1, GameweekPoints = 100, GameweekPointsOnBench = 10, Total = 100 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId1, Gameweek = 2, GameweekPoints = 200, GameweekPointsOnBench = 20, Total = 300 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId2, Gameweek = 1, GameweekPoints = 1, GameweekPointsOnBench = 1, Total = 1 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId2, Gameweek = 2, GameweekPoints = 2, GameweekPointsOnBench = 2, Total = 3 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId3, Gameweek = 1, GameweekPoints = 10, GameweekPointsOnBench = 5, Total = 10 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = oldPlayerId3, Gameweek = 2, GameweekPoints = 20, GameweekPointsOnBench = 6, Total = 30 });


        var stats = await UnitOfWork.Stats.GetMostBenchPointsInAGameweek(oldSeasonId, 1);
        Assert.AreEqual(6, stats.Count());
        Assert.AreEqual("Fake Team 1", stats[0]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[0]["Gameweek"]);
        Assert.AreEqual(20, (int)stats[0]["Points"]);
        Assert.AreEqual("Fake Team 1", stats[1]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[1]["Gameweek"]);
        Assert.AreEqual(10, (int)stats[1]["Points"]);
        Assert.AreEqual("Fake Team 3", stats[2]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[2]["Gameweek"]);
        Assert.AreEqual(6, (int)stats[2]["Points"]);
        Assert.AreEqual("Fake Team 3", stats[3]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[3]["Gameweek"]);
        Assert.AreEqual(5, (int)stats[3]["Points"]);
        Assert.AreEqual("Fake Team 2", stats[4]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[4]["Gameweek"]);
        Assert.AreEqual(2, (int)stats[4]["Points"]);
        Assert.AreEqual("Fake Team 2", stats[5]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[5]["Gameweek"]);
        Assert.AreEqual(1, (int)stats[5]["Points"]);

        // season 2
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        var leagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = 1, SeasonId = seasonId, Name = "PSL" });

        var playerId1 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = leagueId, EntryId = 1, PlayerName = "Fake Player 1", TeamName = "Fake Team 1" });
        var playerId2 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = leagueId, EntryId = 2, PlayerName = "Fake Player 2", TeamName = "Fake Team 2" });
        var playerId3 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = leagueId, EntryId = 3, PlayerName = "Fake Player 3", TeamName = "Fake Team 3" });

        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId1, Gameweek = 1, GameweekPoints = 5, GameweekPointsOnBench = 10, Total = 5 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId1, Gameweek = 2, GameweekPoints = 6, GameweekPointsOnBench = 20, Total = 11 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId2, Gameweek = 1, GameweekPoints = 50, GameweekPointsOnBench = 20, Total = 50 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId2, Gameweek = 2, GameweekPoints = 60, GameweekPointsOnBench = 30, Total = 110 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId3, Gameweek = 1, GameweekPoints = 500, GameweekPointsOnBench = 40, Total = 500 });
        await UnitOfWork.Points.Insert(new Database.Models.Points() { PlayerId = playerId3, Gameweek = 2, GameweekPoints = 600, GameweekPointsOnBench = 50, Total = 1100 });

        stats = await UnitOfWork.Stats.GetMostBenchPointsInAGameweek(seasonId, 1);
        Assert.AreEqual(6, stats.Count());
        Assert.AreEqual("Fake Team 3", stats[0]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[0]["Gameweek"]);
        Assert.AreEqual(50, (int)stats[0]["Points"]);
        Assert.AreEqual("Fake Team 3", stats[1]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[1]["Gameweek"]);
        Assert.AreEqual(40, (int)stats[1]["Points"]);
        Assert.AreEqual("Fake Team 2", stats[2]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[2]["Gameweek"]);
        Assert.AreEqual(30, (int)stats[2]["Points"]);
        Assert.AreEqual("Fake Team 2", stats[3]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[3]["Gameweek"]);
        Assert.AreEqual(20, (int)stats[3]["Points"]);
        Assert.AreEqual("Fake Team 1", stats[4]["Team Name"].ToString());
        Assert.AreEqual(2, (int)stats[4]["Gameweek"]);
        Assert.AreEqual(20, (int)stats[4]["Points"]);
        Assert.AreEqual("Fake Team 1", stats[5]["Team Name"].ToString());
        Assert.AreEqual(1, (int)stats[5]["Gameweek"]);
        Assert.AreEqual(10, (int)stats[5]["Points"]);
    }

    [TestMethod]
    public async Task StatsMostCaptainPointsTest()
    {
        // season 1
        var seasonId = await UnitOfWork.Seasons.Insert(new Database.Models.Season() { Year = "2022/23" });
        var leagueId = await UnitOfWork.Leagues.Insert(new Database.Models.League() { LeagueId = 1, SeasonId = seasonId, Name = "PSL" });

        var playerId1 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = leagueId, EntryId = 1, PlayerName = "Fake Player 1", TeamName = "Fake Team 1" });
        var playerId2 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = leagueId, EntryId = 2, PlayerName = "Fake Player 2", TeamName = "Fake Team 2" });
        var playerId3 = await UnitOfWork.Players.Insert(new Database.Models.Player() { LeagueId = leagueId, EntryId = 3, PlayerName = "Fake Player 3", TeamName = "Fake Team 3" });

        var teamId = await UnitOfWork.Teams.Insert(new Database.Models.Team() { TeamId = 1, SeasonId = seasonId });
        var elementId1 = await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 1, TeamId = teamId });
        var elementId2 = await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 2, TeamId = teamId });
        var elementId3 = await UnitOfWork.Elements.Insert(new Database.Models.Element() { ElementId = 3, TeamId = teamId });

        await UnitOfWork.ElementStats.Insert(new Database.Models.ElementStat() { ElementId = elementId1, Gameweek = 1, TotalPoints = 5});
        await UnitOfWork.ElementStats.Insert(new Database.Models.ElementStat() { ElementId = elementId2, Gameweek = 1, TotalPoints = 1 });
        await UnitOfWork.ElementStats.Insert(new Database.Models.ElementStat() { ElementId = elementId3, Gameweek = 1, TotalPoints = 20 });

        await UnitOfWork.ElementStats.Insert(new Database.Models.ElementStat() { ElementId = elementId1, Gameweek = 2, TotalPoints = 1 });
        await UnitOfWork.ElementStats.Insert(new Database.Models.ElementStat() { ElementId = elementId2, Gameweek = 2, TotalPoints = 5 });
        await UnitOfWork.ElementStats.Insert(new Database.Models.ElementStat() { ElementId = elementId3, Gameweek = 2, TotalPoints = 10 });

        await UnitOfWork.Picks.Insert(new Database.Models.Pick() { ElementId = elementId1, Gameweek = 1, Multiplier = 2, PlayerId = playerId1, Position = 2 });
        await UnitOfWork.Picks.Insert(new Database.Models.Pick() { ElementId = elementId2, Gameweek = 1, Multiplier = 3, PlayerId = playerId2, Position = 3 });
        await UnitOfWork.Picks.Insert(new Database.Models.Pick() { ElementId = elementId3, Gameweek = 1, Multiplier = 1, PlayerId = playerId3, Position = 4 });

        await UnitOfWork.Picks.Insert(new Database.Models.Pick() { ElementId = elementId1, Gameweek = 2, Multiplier = 1, PlayerId = playerId1, Position = 2 });
        await UnitOfWork.Picks.Insert(new Database.Models.Pick() { ElementId = elementId2, Gameweek = 2, Multiplier = 1, PlayerId = playerId2, Position = 3 });
        await UnitOfWork.Picks.Insert(new Database.Models.Pick() { ElementId = elementId3, Gameweek = 2, Multiplier = 3, PlayerId = playerId3, Position = 4 });

        var stats = await UnitOfWork.Stats.GetMostCaptainPoints(seasonId, 1);
        Assert.AreEqual(3, stats.Count());
        Assert.AreEqual("Fake Team 3", stats[0]["Team Name"].ToString());
        Assert.AreEqual(30, (int)stats[0]["Points"]);
        Assert.AreEqual("Fake Team 1", stats[1]["Team Name"].ToString());
        Assert.AreEqual(10, (int)stats[1]["Points"]);
        Assert.AreEqual("Fake Team 2", stats[2]["Team Name"].ToString());
        Assert.AreEqual(3, (int)stats[2]["Points"]);
    }
}
