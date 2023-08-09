using MainCore;
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
                services.AddDbContextFactory<AppDbContext>(options =>
                {
                    var host = hostContext.Configuration["Host"];
                    var port = hostContext.Configuration["Port"];
                    var username = hostContext.Configuration["Username"];
                    var password = hostContext.Configuration["Password"];
                    var worldUrl = hostContext.Configuration["WorldUrl"];
                    var connectionString = $"server={host};port={port};user={username};password={password};database={worldUrl}";
                    options.UseMySQL(connectionString);
                });
                services.AddHttpClient<IGetFileService, GetFileService>();
                services.AddTransient<IParseService, ParseService>();
                services.AddTransient<IUpdateDatabaseService, UpdateDatabaseService>();

                services.AddHostedService<StartUpService>();
            });
    }
}