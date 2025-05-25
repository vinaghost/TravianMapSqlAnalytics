using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContexts
{
    public class ServerDbContext(DbContextOptions<ServerDbContext> options)
        : DbContext(options)
    {
        public DbSet<Server> Servers { get; set; }
    }
}