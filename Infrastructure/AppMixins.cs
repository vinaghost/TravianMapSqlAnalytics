using Application.Models.Options;
using Infrastructure.DbContexts;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure
{
    public static class AppMixins
    {
        public static void ConfigureInfrastructure(this IHostApplicationBuilder builder)
        {
            builder.BindConfiguration();
            builder.ConfigureDbContext();
        }

        private static void BindConfiguration(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(nameof(ConnectionStrings)));
        }

        private static void ConfigureDbContext(this IHostApplicationBuilder builder)
        {
            var isDevelopment = builder.Environment.IsDevelopment();

            builder.Services.AddDbContext<ServerDbContext>((serviceProvider, options) =>
            {
                var connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>().Value;
                options
                    .EnableSensitiveDataLogging(isDevelopment)
                    .EnableDetailedErrors(isDevelopment)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseMySql(connectionStrings.Server, ServerVersion.AutoDetect(connectionStrings.Server));
            });

            builder.Services.AddScoped<IServerCache, ServerCache>();

            builder.Services.AddDbContext<VillageDbContext>((serviceProvider, options) =>
            {
                var serverCache = serviceProvider.GetRequiredService<IServerCache>();
                if (serverCache.Server is null)
                {
                    throw new InvalidOperationException("Server cache is not initialized.");
                }
                var connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>().Value;

                var connectionString = $"{connectionStrings.Village};Database={serverCache.Server}";
                options
                    .EnableSensitiveDataLogging(isDevelopment)
                    .EnableDetailedErrors(isDevelopment)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), x =>
                    {
                        x.EnableIndexOptimizedBooleanColumns();
                        x.EnablePrimitiveCollectionsSupport();
                    });
            });
        }
    }
}