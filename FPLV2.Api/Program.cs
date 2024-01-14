using FPLV2.Client;
using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Database.Repositories;

namespace FPLV2.Api;

public class Program
{
    public static void Main(string[] args)
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
        builder.Configuration.AddEnvironmentVariables();
        var url = builder.Configuration.GetValue<string>("FplBaseUrl") ?? string.Empty;
        builder.Services.AddHttpClient<FplClient>(x => x.BaseAddress = new Uri(url));
        builder.Services.AddOutputCache(options =>
        {
            options.AddPolicy("ExpiryDay", builder =>
                builder.Expire(TimeSpan.FromDays(1)));
        });

        builder.Services.AddControllers();
        var app = builder.Build();
        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();
        app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        app.UseOutputCache();
        app.MapControllers();
        app.Run();
    }
}