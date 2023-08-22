using MainCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MainCore
{
    public class ServerDbContext : DbContext
    {
        public DbSet<Alliance> Alliances { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<VillagePopulation> VillagesPopulations { get; set; }
        public DbSet<PlayerAlliance> PlayersAlliances { get; set; }

        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
        {
        }

        public static string GetConnectionString(IConfiguration configuration, string worldUrl)
        {
            var (host, port, username, password) = (configuration["Database:Host"], configuration["Database:Port"], configuration["Database:Username"], configuration["Database:Password"]);
            var connectionString = $"server={host};port={port};user={username};password={password};database={worldUrl}";
            return connectionString;
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
}