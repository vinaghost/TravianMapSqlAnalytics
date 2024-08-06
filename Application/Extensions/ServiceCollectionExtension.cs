using Application.Models.Options;
using Application.Services;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Application.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCore(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMemoryCache();
            serviceCollection.AddDbContext();
            serviceCollection.AddServices();
            return serviceCollection;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<ServerDbContext>((serviceProvider, options) =>
            {
                var connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>().Value;
                options
#if DEBUG
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
#endif
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseMySql(connectionStrings.Server, ServerVersion.AutoDetect(connectionStrings.Server));
            });

            serviceCollection.AddDbContext<VillageDbContext>((serviceProvider, options) =>
            {
                var connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>().Value;
                var dataService = serviceProvider.GetRequiredService<DataService>();

                var connectionString = $"{connectionStrings.Village};Database={dataService.Server}";
                options
#if DEBUG
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
#endif
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), x =>
                    {
                        x.EnableIndexOptimizedBooleanColumns();
                        x.EnablePrimitiveCollectionsSupport();
                    });
            });

            return serviceCollection;
        }

        public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<CacheService>();
            serviceCollection.TryAddScoped<DataService>();
            return serviceCollection;
        }
    }
}