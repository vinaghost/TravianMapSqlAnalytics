﻿using MapSqlQuery.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace MapSqlQuery
{
    public class AppDbContext : DbContext
    {
        public DbSet<Alliance> Alliances { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<VillagePopulation> VillagesPopulations { get; set; }
        public DbSet<PlayerAlliance> PlayersAlliances { get; set; }
        public DbSet<TimeUpdate> TimeUpdate { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}