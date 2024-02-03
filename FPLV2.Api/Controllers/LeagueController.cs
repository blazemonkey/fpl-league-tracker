using FPLV2.Database.Models;
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
}