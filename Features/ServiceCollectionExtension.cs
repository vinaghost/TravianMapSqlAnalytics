using Features.Shared.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Features
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddFeatures(this IServiceCollection serviceCollection)
        {
            var assembly = typeof(ServiceCollectionExtension).Assembly;

            serviceCollection.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);

                cfg.AddOpenBehavior(typeof(QueryCachingPipelineBehavior<,>));
            });
            serviceCollection.AddValidatorsFromAssembly(assembly);
            return serviceCollection;
        }
    }
}