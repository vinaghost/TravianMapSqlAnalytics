using Core.Entities;
using Microsoft.EntityFrameworkCore;
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

        public ServerDbContext(string dataSource, string database) : base(new DbContextOptionsBuilder().GettOptionsBuilder(dataSource, database).Options)
        {
        }

        public ServerDbContext(string connectionString, ServerVersion version) : base(new DbContextOptionsBuilder().GettOptionsBuilder(connectionString, version).Options)
        {
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
        public static DbContextOptionsBuilder GettOptionsBuilder(this DbContextOptionsBuilder optionsBuilder, string connectionString, ServerVersion version)
        {
            optionsBuilder
                .UseMySql(connectionString, version);

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder GettOptionsBuilder(this DbContextOptionsBuilder optionsBuilder, string dataSource, string database)
        {
            var connectionString = $"{dataSource};Database={database}";
            optionsBuilder
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return optionsBuilder;
        }
    }
}