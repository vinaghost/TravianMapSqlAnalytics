using MapSqlAspNetCoreMVC.Repositories.Implementations;
using MapSqlAspNetCoreMVC.Repositories.Interfaces;
using MapSqlAspNetCoreMVC.Services.Implementations;
using MapSqlAspNetCoreMVC.Services.Interfaces;

namespace MapSqlAspNetCoreMVC.Middlewares.Extension
{
    public static class AppServiceExtension
    {
        public static IServiceCollection AddAppService(this IServiceCollection services)
        {
            services.AddTransient<IDataProvide, DataProvide>();
            services.AddTransient<IPlayerWithPopulationRepository, PlayerWithPopulationRepository>();
            services.AddTransient<IPlayerWithVillagePopulationRepository, PlayerWithVillagePopulationRepository>();
            services.AddTransient<IVillageRepository, VillageRepository>();
            return services;
        }
    }
}