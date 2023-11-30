using MainCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MainCore
{
    public class ServerListDbContext : DbContext
    {
        private const string DATABASE_NAME = "ServerList";
        public DbSet<Server> Servers { get; set; }

        public ServerListDbContext(DbContextOptions<ServerListDbContext> options) : base(options)
        {
        }

        public static string GetConnectionString(IConfiguration configuration)
        {
            var (host, port, username, password) = (configuration["Host"], configuration["Port"], configuration["Username"], configuration["Password"]);
            var connectionString = $"Server={host};Port={port};Uid={username};Pwd={password};Database={DATABASE_NAME}";
            return connectionString;
        }
    }
}