using Core;
using MapSqlDatabaseUpdate.Commands;
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
#if DEBUG
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors()
#endif
                        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                });
                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                });

                services.AddHttpClient<GetServerListCommandHandler>();
                services.AddHttpClient<GetMapSqlCommandHandler>();

                services.AddHostedService<StartUpService>();
            });
    }
}