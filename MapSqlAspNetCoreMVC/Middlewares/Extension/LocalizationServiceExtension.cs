using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace MapSqlAspNetCoreMVC.Middlewares.Extension
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

                var requestProvider = options.RequestCultureProviders.OfType<AcceptLanguageHeaderRequestCultureProvider>().First();
                options.RequestCultureProviders.Remove(requestProvider);
            });

            services.AddScoped<RequestLocalizationCookiesMiddleware>();
            return services;
        }
    }
}