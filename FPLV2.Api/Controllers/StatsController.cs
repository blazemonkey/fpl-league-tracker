using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FPLV2.Api.Controllers;

[ApiController]
[Route("stats")]
public class StatsController : BaseController
{

    public StatsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    /// <summary>
    /// Get all stats
    /// </summary>
    /// <returns>An array of stats</returns>
    [HttpGet]
    public async Task<Stats[]> Get()
    {
        var stats = await UnitOfWork.Stats.GetAll();
        return stats.OrderBy(x => x.DisplayOrder).ToArray();
    }

    /// <summary>
    /// Get overall stats details
    /// </summary>
    /// <param name="id">Id of the stats</param>
    /// <param name="seasonId">Id of the season</param>
    /// <param name="leagueId">Id of the league</param>
    /// <returns>Data for the stats</returns>
    [HttpGet("overall/{id}/{seasonId}/{leagueId}")]
    public async Task<List<IDictionary<string, object>>> GetLineChart([FromRoute] int id, [FromRoute] int seasonId, [FromRoute] int leagueId)
    {
        var stats = await UnitOfWork.Stats.GetById(id);
        if (stats == null)
            return null;

        var details = await UnitOfWork.Stats.GetOverallStatsDetails(stats.Name, seasonId, leagueId);
        return details;
    }

    /// <summary>
    /// Get team stats details
    /// </summary>
    /// <param name="id">Id of the stats</param>
    /// <param name="seasonId">Id of the season</param>
    /// <param name="leagueId">Id of the league</param>
    /// <param name="playerId">Id of the player</param>
    /// <returns>Data for the stats</returns>
    [HttpGet("team/{id}/{seasonId}/{leagueId}/{playerId}")]
    public async Task<List<IDictionary<string, object>>> GetPointsChart([FromRoute] int id, [FromRoute] int seasonId, [FromRoute] int leagueId, [FromRoute] int playerId)
    {
        var stats = await UnitOfWork.Stats.GetById(id);
        if (stats == null)
            return null;

        var details = await UnitOfWork.Stats.GetTeamStatsDetails(stats.Name, seasonId, leagueId, playerId);
        return details;
    }
}