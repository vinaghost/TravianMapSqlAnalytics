using Microsoft.Extensions.DependencyInjection.Extensions;
using WebAPI.Services;

namespace WebAPI.Extensions
{
    public static class ServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<DataService>();
            serviceCollection.TryAddSingleton<ICacheService, CacheService>();
            return serviceCollection;
        }
    }
}