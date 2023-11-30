using FPLV2.Api.Models;
using FPLV2.Client;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories;
using FPLV2.Database.Repositories.Interfaces;

internal class Program
{
    private static LeagueSearch[] LeagueNames { get; set; }

    private static async Task Main(string[] args)
    {
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

        builder.Services.AddCors();
        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        var url = builder.Configuration.GetValue<string>("FplBaseUrl") ?? string.Empty;
        builder.Services.AddHttpClient<FplClient>(x => x.BaseAddress = new Uri(url));
        builder.Services.AddOutputCache(options =>
        {
            options.AddPolicy("ExpiryDay", builder =>
                builder.Expire(TimeSpan.FromDays(1)));
        });

        var app = builder.Build();
        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();
        app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        app.UseOutputCache();
        var unitOfWork = app.Services.GetService<IUnitOfWork>();
        if (unitOfWork != null)
        {
            var result = await unitOfWork.LeagueSearch.GetAll();
            var names = result.Where(x => x.LeagueType == "x");
            LeagueNames = names.ToArray();
        }


        #region Leagues
        // Check if this league id exists and return the name so it can be confirmed on the client side
        app.MapGet("/leagues/{leagueId}/check", async (HttpContext context, FplClient fplClient, int leagueId) =>
        {
            try
            {
                var league = await fplClient.GetLeagueStandings(leagueId, true);
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

            var league = await fplClient.GetLeagueStandings(leagueId, true);
            return false;
        });

        // Get league summary from database, grab from api and insert if doesn't exist
        app.MapGet("/leagues/{seasonId}/{leagueId}/summary", async (IUnitOfWork unitOfWork, int seasonId, int leagueId) =>
        {
            var leagues = await unitOfWork.Leagues.GetAllBySeasonId(seasonId);
            var league = leagues.FirstOrDefault(x => x.LeagueId == leagueId);
            if (league == null)
            {

            }

            var leagueSummary = new LeagueSummary()
            {
                Id = league.LeagueId,
                Name = league.Name
            };

            return leagueSummary;
        });
        #endregion

        #region Leagues Search
        // Get list of leagues that match an ID or name
        app.MapGet("/leagues/search", async (IUnitOfWork unitOfWork, string name) =>
        {
            var results = new List<LeagueSearch>();

            var isId = int.TryParse(name, out var id);
            if (isId)
            {
                var league = LeagueNames.FirstOrDefault(x => x.LeagueId == id);
                if (league != null)
                    results.Add(league);
            }
            else
            {
                var leagues = LeagueNames.Where(x => x.Name.ToLower().Contains(name.ToLower()));
                results.AddRange(leagues);
            }

            return results;
        });
        #endregion

        #region Elements
        app.MapGet("/elements/{seasonId}", async (IUnitOfWork unitOfWork, int seasonId) =>
        {
            var teams = await unitOfWork.Elements.GetAllBySeasonId(seasonId);
            return teams.ToArray();
        });
        #endregion

        #region Element Stats
        app.MapGet("/element-stats/{seasonId}", async (IUnitOfWork unitOfWork, int seasonId) =>
        {
            var teams = await unitOfWork.ElementStats.GetAllBySeasonId(seasonId);
            return teams.ToArray();
        });
        #endregion

        #region Players
        app.MapGet("/players/{seasonId}/{leagueId}", async (IUnitOfWork unitOfWork, int seasonId, int leagueId) =>
        {
            var leagues = await unitOfWork.Leagues.GetAllBySeasonId(seasonId);
            var league = leagues.FirstOrDefault(x => x.LeagueId == leagueId);
            if (league == null)
                return null;

            var players = await unitOfWork.Players.GetAllByLeagueId(league.Id);
            return players.ToArray();
        });
        #endregion

        #region Teams
        app.MapGet("/teams/{seasonId}", async (IUnitOfWork unitOfWork, int seasonId) =>
        {
            var teams = await unitOfWork.Teams.GetAllBySeasonId(seasonId);
            return teams.ToArray();
        });
        #endregion

        #region
        app.MapGet("/seasons/latest", async (IUnitOfWork unitOfWork) =>
        {
            var seasons = await unitOfWork.Seasons.GetAll();
            var latest = seasons.OrderByDescending(x => x.Id).FirstOrDefault();
            return latest;
        }).CacheOutput("ExpiryDay");
        #endregion

        #region Stats
        // Get all Stats
        app.MapGet("/stats", async (IUnitOfWork unitOfWork, IConfiguration configuration) =>
        {
            var stats = await unitOfWork.Stats.GetAll();
            return stats.OrderBy(x => x.DisplayOrder).ToArray();
        });

        // Get overall Stats details by Id
        app.MapGet("/stats/overall/{id}/{seasonId}/{leagueId}", async (IUnitOfWork unitOfWork, int id, int seasonId, int leagueId) =>
        {
            var stats = await unitOfWork.Stats.GetById(id);
            if (stats == null)
                return null;

            var details = await unitOfWork.Stats.GetOverallStatsDetails(stats.Name, seasonId, leagueId);
            return details;
        });
        #endregion

        #region Charts
        // Get all Charts
        app.MapGet("/charts", async (IUnitOfWork unitOfWork) =>
        {
            var stats = await unitOfWork.Charts.GetAll();
            return stats.OrderBy(x => x.DisplayOrder).ToArray();
        });

        // Get line Chart details by Id
        app.MapGet("/charts/line/{id}/{seasonId}/{leagueId}", async (IUnitOfWork unitOfWork, int id, int seasonId, int leagueId) =>
        {
            var c = await unitOfWork.Charts.GetById(id);
            if (c == null)
                return null;

            var chart = await unitOfWork.Charts.GetLineChart(c.Name, seasonId, leagueId);
            return chart;
        });

        // Get the Points Chart
        app.MapPost("/charts/points/{seasonId}/{leagueId}", async (IUnitOfWork unitOfWork, int seasonId, int leagueId, PointsChartOptions options) =>
        {
            var chart = await unitOfWork.Charts.GetPointsChart(seasonId, leagueId, options);
            return chart;
        });
        #endregion

        app.Run();
    }
}

#region Leagues

#endregion
#region Leagues Search

#endregion
#region Elements

#endregion
#region Element Stats

#endregion
#region Players

#endregion
#region Teams

#endregion
#region

#endregion
#region Stats

#endregion
#region Charts

#endregion
