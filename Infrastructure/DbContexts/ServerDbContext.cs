using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContexts
{
    public class ServerDbContext(DbContextOptions<ServerDbContext> options)
        : DbContext(options)
    {
        public DbSet<Server> Servers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Server>()
                .Property(b => b.LastUpdate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Server>()
                .Property(b => b.AllianceCount)
                .HasColumnType("int")
                .HasDefaultValueSql("0");

            modelBuilder.Entity<Server>()
                .Property(b => b.PlayerCount)
                .HasColumnType("int")
                .HasDefaultValueSql("0");

            modelBuilder.Entity<Server>()
                .Property(b => b.VillageCount)
                .HasColumnType("int")
                .HasDefaultValueSql("0");
        }
    }
}