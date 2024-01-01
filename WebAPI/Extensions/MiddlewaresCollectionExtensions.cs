using Microsoft.Extensions.DependencyInjection.Extensions;
using WebAPI.Middlewares;

namespace WebAPI.Extensions
{
    public static class MiddlewaresServiceCollectionExtensions
    {
        public static IServiceCollection AddMiddleware(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<ServerMiddleware>();
            return serviceCollection;
        }
    }
}