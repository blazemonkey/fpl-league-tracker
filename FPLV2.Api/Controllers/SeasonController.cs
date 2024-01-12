using FPLV2.Database.Models;
using FPLV2.Database.Repositories;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FPLV2.Api.Controllers;

[ApiController]
[Route("seasons")]
public class SeasonController : BaseController
{

    public SeasonController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    /// <summary>
    /// Get the latest season in the database
    /// </summary>
    /// <returns>Season object</returns>
    [HttpGet("latest")]
    public async Task<Season> GetLatest()
    {
        var seasons = await UnitOfWork.Seasons.GetAll();
        var latest = seasons.OrderByDescending(x => x.Id).FirstOrDefault();
        return latest;
    }
}