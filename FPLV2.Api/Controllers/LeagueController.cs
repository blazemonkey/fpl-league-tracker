using FPLV2.Api.Models;
using FPLV2.Client.Models;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FPLV2.Api.Controllers;

[ApiController]
[Route("leagues")]
public class LeagueController : BaseController
{

    public LeagueController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    /// <summary>
    /// Get list of leagues that match an ID or name
    /// </summary>
    /// <param name="seasonId">Id of the season</param>
    /// <param name="leagueIdOrName">ID or name of the league</param>
    /// <returns>An array of players</returns>
    [HttpGet("search/{seasonId}")]
    public async Task<LeagueSearch[]> Search([FromRoute] int seasonId, [FromQuery] string leagueIdOrName)
    {
        var results = new List<LeagueSearch>();

        var isId = int.TryParse(leagueIdOrName, out var id);
        if (isId)
        {
            var league = await UnitOfWork.LeagueSearch.GetLeagueBySeasonIdAndLeagueId(seasonId, id);
            if (league != null)
                results.Add(league);
        }
        else
        {
            var leagues = await UnitOfWork.LeagueSearch.GetLeagueBySeasonIdAndLeagueName(seasonId, leagueIdOrName);
            results.AddRange(leagues);
        }

        return results.ToArray();
    }

    /// <summary>
    /// Get summary for a league
    /// </summary>
    /// <param name="seasonId">Id of the season</param>
    /// <param name="leagueId">Id of the league</param>
    /// <returns>An array of players</returns>
    [HttpGet("{seasonId}/{leagueId}/summary")]
    public async Task<LeagueSummary> GetSummary([FromRoute] int seasonId, [FromRoute] int leagueId)
    {
        var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(seasonId);
        var league = leagues.FirstOrDefault(x => x.LeagueId == leagueId);
        if (league == null)
        {
        }

        var players = await UnitOfWork.Players.GetAllByLeagueId(league.Id);
        var teams = await UnitOfWork.Teams.GetAllBySeasonId(seasonId);
        var leagueSummary = new LeagueSummary()
        {
            Id = league.LeagueId,
            Name = league.Name,
            Players = players.OrderBy(x => x.TeamName).ToArray(),
            Teams = teams.ToArray()
        };

        return leagueSummary;
    }
}