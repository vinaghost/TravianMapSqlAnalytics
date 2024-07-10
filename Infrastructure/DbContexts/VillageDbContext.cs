using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContexts
{
    public class VillageDbContext(DbContextOptions<VillageDbContext> options)
        : DbContext(options)
    {
        public DbSet<Alliance> Alliances { get; set; }
        public DbSet<AllianceHistory> AlliancesHistory { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerHistory> PlayersHistory { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<VillageHistory> VillagesHistory { get; set; }
    }
}