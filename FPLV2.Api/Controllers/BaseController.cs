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
}