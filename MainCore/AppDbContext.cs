using MainCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MainCore
{
    public class AppDbContext : DbContext
    {
        public DbSet<Alliance> Alliances { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<VillagePopulation> VillagesPopulations { get; set; }
        public DbSet<PlayerAlliance> PlayersAlliances { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public static string GetConnectionString(IConfiguration configuration, string worldUrl)
        {
            var connectionString = "";
            return connectionString;
        }
    }
}