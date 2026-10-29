using System.Data;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MySqlConnector;

using Serilog;

namespace Infrastructure
{
    public enum DatabaseConnectionType
    {
        Villages,
        Servers
    }

    public static class AppMixins
    {
        public static void ConfigureInfrastructure(this IHostApplicationBuilder builder)
        {
            builder.ConfigureLogging();
            builder.ConfigureDbConnection();
        }

        public static void ConfigureLogging(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSerilog(c => c
                    .ReadFrom.Configuration(builder.Configuration));
        }

        private static void ConfigureDbConnection(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<ServerCache>();

            builder.Services.AddSingleton(serviceProvider =>
            {
                var dataSourceBuilder = new MySqlDataSourceBuilder(builder.Configuration["DatabaseConnectionString"]!)
                            .UseLoggerFactory(serviceProvider.GetService<ILoggerFactory>());
                return dataSourceBuilder.Build();
            });

            builder.Services.AddKeyedScoped<IDbConnection>(DatabaseConnectionType.Villages, (serviceProvider, serviceKey) =>
            {
                if (serviceKey is not string stringKey)
                {
                    throw new ArgumentException("Invalid service key type provided.");
                }

                var serverCache = serviceProvider.GetRequiredService<ServerCache>();
                if (serverCache.Server is null)
                {
                    throw new InvalidOperationException("Server cache is not initialized.");
                }
                var dataSource = serviceProvider.GetRequiredService<MySqlDataSource>();
                var connection = dataSource.CreateConnection();
                connection.ChangeDatabase(serverCache.Server);
                return connection;
            });

            builder.Services.AddKeyedScoped<IDbConnection>(DatabaseConnectionType.Servers, (serviceProvider, serviceKey) =>
            {
                if (serviceKey is not string stringKey)
                {
                    throw new ArgumentException("Invalid service key type provided.");
                }
                var dataSource = serviceProvider.GetRequiredService<MySqlDataSource>();
                var connection = dataSource.CreateConnection();
                connection.ChangeDatabase("Servers");
                return connection;
            });
        }
    }
}