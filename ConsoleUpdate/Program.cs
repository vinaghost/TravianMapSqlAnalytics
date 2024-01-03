using ConsoleUpdate.Commands;
using ConsoleUpdate.Services;
using Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddCore();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
        services.AddHttpClient<GetServerListCommandHandler>();
        services.AddHttpClient<GetMapSqlCommandHandler>();

        services.AddHostedService<StartUpService>();
    })
    .RunConsoleAsync();