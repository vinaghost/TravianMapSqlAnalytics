﻿using Core.Behaviors;
using Core.Config;
using Core.Repositories;
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

            serviceCollection.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtension).Assembly);

                cfg.AddOpenBehavior(typeof(QueryCachingPipelineBehavior<,>));
            });

            serviceCollection.AddDbContext();
            serviceCollection.AddRepository();
            serviceCollection.AddServices();
            return serviceCollection;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<ServerListDbContext>((serviceProvider, options) =>
            {
                var option = serviceProvider.GetRequiredService<IOptions<ConnectionStringOption>>();

                var connectionString = $"{option.Value.Value};Database={ServerListDbContext.DATABASE_NAME}";
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

                var connectionString = $"{option.Value.Value};Database={dataService.Server}";
                options
#if DEBUG
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
#endif
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            return serviceCollection;
        }

        public static IServiceCollection AddRepository(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient<UnitOfRepository>();

            serviceCollection.TryAddTransient<IServerRepository, ServerRepository>();
            serviceCollection.TryAddTransient<IVillageRepository, VillageRepository>();
            serviceCollection.TryAddTransient<IPlayerRepository, PlayerRepository>();
            serviceCollection.TryAddTransient<IAllianceRepository, AllianceRepository>();
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