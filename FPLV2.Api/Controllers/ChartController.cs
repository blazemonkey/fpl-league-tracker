using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FPLV2.Api.Controllers;

[ApiController]
[Route("charts")]
public class ChartController : BaseController
{

    public ChartController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    /// <summary>
    /// Get all charts
    /// </summary>
    /// <returns>An array of charts</returns>
    [HttpGet]
    public async Task<Chart[]> Get()
    {
        var charts = await UnitOfWork.Charts.GetAll();
        return charts.OrderBy(x => x.DisplayOrder).ToArray();
    }

    /// <summary>
    /// Get a line chart
    /// </summary>
    /// <param name="id">Id of the chart</param>
    /// <param name="seasonId">Id of the season</param>
    /// <param name="leagueId">Id of the league</param>
    /// <returns>Data for the line chart</returns>
    [HttpGet("line/{id}/{seasonId}/{leagueId}")]
    public async Task<IDictionary<string, LineChartPoint[]>> GetLineChart([FromRoute] int id, [FromRoute] int seasonId, [FromRoute] int leagueId)
    {
        var c = await UnitOfWork.Charts.GetById(id);
        if (c == null)
            return null;

        var chart = await UnitOfWork.Charts.GetLineChart(c.Name, seasonId, leagueId);
        return chart;
    }

    /// <summary>
    /// Get the points chart
    /// </summary>
    /// <param name="seasonId">Id of the season</param>
    /// <param name="leagueId">Id of the league</param>
    /// <param name="options">Custom options of the chart</param>
    /// <returns>Data for the line chart</returns>
    [HttpPost("points/{seasonId}/{leagueId}")]
    public async Task<PointsChartGroupedData[]> GetPointsChart([FromRoute] int seasonId, [FromRoute] int leagueId, [FromBody] PointsChartOptions options)
    {
        var chart = await UnitOfWork.Charts.GetPointsChart(seasonId, leagueId, options);
        return chart;
    }
}