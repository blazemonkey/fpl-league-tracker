using FPLV2.Client;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var types = typeof(BaseRepository).Assembly.GetTypes().Where(x => x.BaseType == typeof(BaseRepository));
foreach (var type in types)
{
    var interfaceType = type.GetInterface($"I{type.Name}");
    if (interfaceType == null)
        continue;

    builder.Services.AddTransient(interfaceType, type);
}

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
var url = builder.Configuration.GetValue<string>("FplBaseUrl") ?? string.Empty;
builder.Services.AddHttpClient<FplClient>(x => x.BaseAddress = new Uri(url));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

#region Leagues
// Check if this league id exists and return the name so it can be confirmed on the client side
app.MapGet("/leagues/{leagueId}/check", async (HttpContext context, FplClient fplClient, int leagueId) =>
{
    try
    {        
        var league = await fplClient.GetLeagueStandings(leagueId);
        if (league.League.LeagueType != "x")
            return "";

        return league.League.Name;
    }
    catch (Exception)
    {
        return "";
    }
});

// Add the league id to the database
app.MapPost("/leagues/{leagueId}", async (HttpContext context, IUnitOfWork unitOfWork, FplClient fplClient, int leagueId) => 
{
    var seasons = await unitOfWork.Seasons.GetAll();
    var latestSeason = seasons.LastOrDefault();
    if (latestSeason == null)
        return false;

    var league = await fplClient.GetLeagueStandings(leagueId);
    return false;
});

// Get the points for all the players in the league, this is used for showing charts
app.MapGet("/leagues/{id}/points-history/{type}", async (IUnitOfWork unitOfWork, IConfiguration configuration, int id, string type) =>
{
    PointsHistory[] points = null;
    if (type == "total")
        points = await unitOfWork.Points.GetTotalPointsHistory(id);
    else
        throw new Exception("Type is not valid");

    var results = points.GroupBy(x => x.TeamName).ToDictionary(x => x.Key, x => x.Select(x => new { x.Gameweek, x.Points }).ToArray());
    return results;
});
#endregion

#region Stats
// Get all Stats
app.MapGet("/stats", async (IUnitOfWork unitOfWork, IConfiguration configuration) =>
{
    var stats = await unitOfWork.Stats.GetAll();
    return stats.OrderBy(x => x.DisplayOrder).ToArray();
});

// Get Stats details by Id
app.MapGet("/stats/overall/{id}/{seasonId}/{leagueId}", async (IUnitOfWork unitOfWork, int id, int seasonId, int leagueId) =>
{
    var stats = await unitOfWork.Stats.GetById(id);
    if (stats == null)
        return null;

    var details = await unitOfWork.Stats.GetOverallStatsDetails(stats.Name, seasonId, leagueId);
    return details;
});
#endregion

app.Run();