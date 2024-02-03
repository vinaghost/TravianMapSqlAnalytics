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
        public DbSet<VillagePopulationHistory> VillagePopulationHistory { get; set; }
        public DbSet<PlayerPopulationHistory> PlayerPopulationHistory { get; set; }
        public DbSet<PlayerAllianceHistory> PlayerAllianceHistory { get; set; }

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