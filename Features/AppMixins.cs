using Features.Behaviors;
using Features.Services;
using FluentValidation;
using Immediate.Handlers.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: Behaviors(
    typeof(QueryCachingBehavior<,>)
)]

namespace Features
{
    public static class AppMixins
    {
        public static void ConfigureFeatures(this IHostApplicationBuilder builder)
        {
            builder.ConfigureServices();
        }

        private static void ConfigureServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<CacheService>();
            var assembly = typeof(AppMixins).Assembly;
            builder.Services.AddValidatorsFromAssembly(assembly);

            builder.Services.AddFeaturesHandlers();
            builder.Services.AddFeaturesBehaviors();
        }
    }
}