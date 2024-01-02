using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    public class ServerDbContext : DbContext
    {
        public DbSet<Alliance> Alliances { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<VillagePopulation> VillagesPopulations { get; set; }
        public DbSet<PlayerAlliance> PlayersAlliances { get; set; }

        [ActivatorUtilitiesConstructor]
        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
        {
        }

        public ServerDbContext(IConfiguration configuration, string worldUrl) : base(new DbContextOptionsBuilder().GettOptionsBuilder(configuration, worldUrl).Options)
        {
        }

        public static string GetConnectionString(IConfiguration configuration, string worldUrl)
        {
            var connectionString = $"{GetHost(configuration)};Database={worldUrl}";
            return connectionString;
        }

        public static string GetHost(IConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(configuration["Host"]))
            {
                return $"Server={configuration["Host"]};Port={configuration["Port"]};Uid={configuration["Username"]};Pwd={configuration["Password"]};";
            }
            else
            {
                return $"Server={configuration["Database:Host"]};Port={configuration["Database:Port"]};Uid={configuration["Database:Username"]};Pwd={configuration["Database:Password"]};";
            }
        }

        public List<DateTime> GetDateBefore(int days)
        {
            var dates = new List<DateTime>();
            var today = GetNewestDay();
            for (int i = 0; i <= days; i++)
            {
                var beforeDate = today.AddDays(-i);
                dates.Add(beforeDate);
            }
            return dates;
        }

        public DateTime GetNewestDay()
        {
            var query = VillagesPopulations
                .OrderByDescending(x => x.Date)
                .Select(x => x.Date)
                .FirstOrDefault();
            return query;
        }
    }

    public static class ServerDbContextExtension
    {
        public static DbContextOptionsBuilder GettOptionsBuilder(this DbContextOptionsBuilder optionsBuilder, IConfiguration configuration, string worldUrl)
        {
            var connectionString = ServerDbContext.GetConnectionString(configuration, worldUrl);

            optionsBuilder
#if DEBUG
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
#endif
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return optionsBuilder;
        }
    }
}