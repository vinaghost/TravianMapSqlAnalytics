using MapSqlAspNetCoreMVC.Middlewares;

namespace MapSqlAspNetCoreMVC.Extension
{
    public static class ServerServiceExtension
    {
        public static IServiceCollection AddServerService(this IServiceCollection services)
        {
            services.AddScoped<RequestServerCookiesMiddleware>();
            return services;
        }
    }
}