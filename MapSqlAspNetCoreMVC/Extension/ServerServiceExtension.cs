using MapSqlAspNetCoreMVC.Middlewares;

namespace MapSqlAspNetCoreMVC.Extension
{
    public static class ServerServiceExtension
    {
        public static IServiceCollection AddMiddleware(this IServiceCollection services)
        {
            services.AddScoped<RequestServerCookiesMiddleware>();
            services.AddScoped<RequestServerMiddleware>();
            return services;
        }
    }
}