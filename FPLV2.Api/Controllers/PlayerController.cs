using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FPLV2.Api.Controllers;

[ApiController]
[Route("players")]
public class PlayerController : BaseController
{

    public PlayerController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    /// <summary>
    /// Get all players in a league
    /// </summary>
    /// <param name="seasonId">Id of the season</param>
    /// <param name="leagueId">Id of the league. This is the actual LeagueId from the API</param>
    /// <returns>An array of players</returns>
    [HttpGet("{seasonId}/{leagueId}")]
    public async Task<Player[]> Get([FromRoute] int seasonId, [FromRoute] int leagueId)
    {
        var leagues = await UnitOfWork.Leagues.GetAllBySeasonId(seasonId);
        var league = leagues.FirstOrDefault(x => x.LeagueId == leagueId);
        if (league == null)
            return null;

        var players = await UnitOfWork.Players.GetAllByLeagueId(league.Id);
        return players.ToArray();
    }
}