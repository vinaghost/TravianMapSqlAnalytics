using Core.Config;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Core.Extensions
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
            serviceCollection.AddDbContext<ServerListDbContext>((serviceProvider, options) =>
            {
                var option = serviceProvider.GetRequiredService<IOptions<ConnectionStringOption>>();

                var connectionString = $"{option.Value.DataSource};Database={ServerListDbContext.DATABASE_NAME}";
                options
#if DEBUG
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
#endif
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            serviceCollection.AddDbContext<ServerDbContext>((serviceProvider, options) =>
            {
                var option = serviceProvider.GetRequiredService<IOptions<ConnectionStringOption>>();
                var dataService = serviceProvider.GetRequiredService<DataService>();

                var connectionString = $"{option.Value.DataSource};Database={dataService.Server}";
                options
#if DEBUG
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    //.LogTo(Console.WriteLine)
#endif
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            return serviceCollection;
        }

        public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ICacheService, CacheService>();
            serviceCollection.TryAddScoped<DataService>();
            return serviceCollection;
        }
    }
}