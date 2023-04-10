using FPLV2.Database.Repositories;
using FPLV2.Database.Repositories.Interfaces;

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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

#region Stats
// Get all Stats
app.MapGet("/stats", async (IUnitOfWork unitOfWork) =>
{
    var stats = await unitOfWork.Stats.GetAll();
    return stats;
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