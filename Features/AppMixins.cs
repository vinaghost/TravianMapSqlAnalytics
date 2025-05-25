using Features.Services;
using Features.Shared.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(QueryCachingPipelineBehavior<,>));
            });
        }
    }
}