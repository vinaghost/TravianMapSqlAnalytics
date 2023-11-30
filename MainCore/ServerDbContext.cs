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

        private readonly string _connectionString;

        public ServerDbContext(IConfiguration configuration, string worldUrl)
        {
            _connectionString = GetConnectionString(configuration, worldUrl);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString))
#if DEBUG
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
#endif
        }

        public static string GetConnectionString(IConfiguration configuration, string worldUrl)
        {
            var (host, port, username, password) = (configuration["Host"], configuration["Port"], configuration["Username"], configuration["Password"]);
            var connectionString = $"Server={host};Port={port};Uid={username};Pwd={password};Database={worldUrl}";
            return connectionString;
        }
    }
}