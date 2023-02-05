using FPLV2.Database.Repositories;
using FPLV2.Database.Repositories.Interfaces;
using FPLV2.Updater;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        var types = typeof(BaseRepository).Assembly.GetTypes().Where(x => x.BaseType == typeof(BaseRepository));
        foreach (var type in types)
        {
            var interfaceType = type.GetInterface($"I{type.Name}");
            if (interfaceType == null)
                continue;

            services.AddTransient(interfaceType, type);
        }

        services.AddTransient<IUnitOfWork, UnitOfWork>();

        var url = config.GetValue<string>("FplBaseUrl");
        services.AddHttpClient<FplClient>(x => x.BaseAddress = new Uri(url));
    })
    .Build();

host.Run();