using FPLV2.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FPLV2.Api.Controllers;

/// <summary>
/// Base Controller that all controllers inherit
/// </summary>
[ApiController]
[Route("[controller]")]
public class BaseController: ControllerBase
{
    /// <summary>
    /// Gets or sets the UnitOfWork to call the Database
    /// </summary>
    protected IUnitOfWork UnitOfWork { get; set; }

    public BaseController(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    //[HttpGet]
    //public IEnumerable<WeatherForecast> Get()
    //{
    //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //    {
    //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
    //        TemperatureC = Random.Shared.Next(-20, 55),
    //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //    })
    //    .ToArray();
    //}
}