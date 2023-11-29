using MainCore;
using MapSqlDatabaseUpdate.CQRS.Commands;
using MapSqlDatabaseUpdate.Service.Implementations;
using MapSqlDatabaseUpdate.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MapSqlDatabaseUpdate
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContextFactory<ServerListDbContext>(options =>
                {
                    var connectionString = ServerListDbContext.GetConnectionString(hostContext.Configuration);
                    options
                        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
#if DEBUG
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
#endif
                });
                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                });

                services.AddHttpClient<GetServerListCommandHandler>();

                services.AddHttpClient<IGetFileService, GetFileService>();
                services.AddTransient<IParseService, ParseService>();
                services.AddTransient<IUpdateDatabaseService, UpdateDatabaseService>();

                services.AddHostedService<StartUpService>();
            });
    }
}