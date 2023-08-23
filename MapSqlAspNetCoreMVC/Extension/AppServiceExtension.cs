using MapSqlAspNetCoreMVC.Repositories.Implementations;
using MapSqlAspNetCoreMVC.Repositories.Interfaces;
using MapSqlAspNetCoreMVC.Services.Implementations;
using MapSqlAspNetCoreMVC.Services.Interfaces;

namespace MapSqlAspNetCoreMVC.Extension
{
    public static class AppServiceExtension
    {
        public static IServiceCollection AddAppService(this IServiceCollection services)
        {
            services.AddTransient<IPlayerWithAllianceRepository, PlayerWithAllianceRepository>();
            services.AddTransient<IPlayerWithPopulationRepository, PlayerWithPopulationRepository>();
            services.AddTransient<IPlayerWithDetailRepository, PlayerWithDetailRepository>();
            services.AddTransient<IVillageRepository, VillageRepository>();
            services.AddTransient<IDataProvide, DataProvide>();
            return services;
        }
    }
}