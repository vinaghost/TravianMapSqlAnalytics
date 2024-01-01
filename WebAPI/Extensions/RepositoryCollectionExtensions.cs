using Microsoft.Extensions.DependencyInjection.Extensions;
using WebAPI.Repositories;

namespace WebAPI.Extensions
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient<UnitOfRepository>();

            serviceCollection.TryAddTransient<IServerRepository, ServerRepository>();
            serviceCollection.TryAddTransient<IVillageRepository, VillageRepository>();
            serviceCollection.TryAddTransient<IPlayerRepository, PlayerRepository>();
            serviceCollection.TryAddTransient<IAllianceRepository, AllianceRepository>();
            return serviceCollection;
        }
    }
}