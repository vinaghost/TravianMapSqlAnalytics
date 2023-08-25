using MapSqlAspNetCoreMVC.Middlewares;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace MapSqlAspNetCoreMVC.Extension
{
    public static class LocalizationServiceExtension
    {
        public static IServiceCollection AddLocalizationService(this IServiceCollection services)
        {
            services.TryAddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("vi-VN");
                options.AddSupportedUICultures("vi-VN", "en-US");
                options.FallBackToParentUICultures = true;

                options.RequestCultureProviders.Remove(typeof(AcceptLanguageHeaderRequestCultureProvider));
            });

            services.AddScoped<RequestLocalizationCookiesMiddleware>();
            return services;
        }
    }
}