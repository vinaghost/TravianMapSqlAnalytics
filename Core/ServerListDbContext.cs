using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core
{
    public class ServerListDbContext : DbContext
    {
        public const string DATABASE_NAME = "ServerList";
        public DbSet<Server> Servers { get; set; }

        public ServerListDbContext(DbContextOptions<ServerListDbContext> options) : base(options)
        {
        }

        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = $"{GetHost(configuration)};Database={DATABASE_NAME}";
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
    }

    public static class ServerListDbContextExtension
    {
        public static DbContextOptionsBuilder GettOptionsBuilder(this DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
        {
            var connectionString = ServerListDbContext.GetConnectionString(configuration);

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