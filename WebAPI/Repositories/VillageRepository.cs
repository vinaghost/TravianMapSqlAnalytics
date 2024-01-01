using Core;
using WebAPI.Models.Parameters;
using VillageEntity = Core.Models.Village;

namespace WebAPI.Repositories
{
    public class VillageRepository(ServerDbContext dbContext) : IVillageRepository
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public IQueryable<VillageEntity> GetQueryable(IVillageFilterParameter parameters)
        {
            return GetBaseQueryable(parameters)
                .Where(x => x.Population >= parameters.MinPopulation)
                .Where(x => x.Population <= parameters.MaxPopulation);
        }

        private IQueryable<VillageEntity> GetBaseQueryable(IVillageFilterParameter parameters)
        {
            if (parameters.Alliances.Count == 0 && parameters.Players.Count == 0 && parameters.Villages.Count == 0) return _dbContext.Villages.AsQueryable();

            IQueryable<VillageEntity>? query = null;

            query = GetAlliancesFilterQueryable(query, parameters.Alliances);
            query = GetPlayersFilterQueryable(query, parameters.Players);
            query = GetVillagesFilterQueryable(query, parameters.Villages);

            if (query is null) return _dbContext.Villages.AsQueryable();
            return query;
        }

        private IQueryable<VillageEntity>? GetAlliancesFilterQueryable(IQueryable<VillageEntity>? query, List<int> Alliances)
        {
            if (Alliances.Count == 0) return query;
            var allianceQuery = _dbContext.Alliances
                 .Where(x => Alliances.Contains(x.AllianceId))
                 .SelectMany(x => x.Players)
                 .SelectMany(x => x.Villages);
            if (query is null) return allianceQuery;
            return query
                .Union(allianceQuery);
        }

        private IQueryable<VillageEntity>? GetPlayersFilterQueryable(IQueryable<VillageEntity>? query, List<int> Players)
        {
            if (Players.Count == 0) return query;
            var playerQuery = _dbContext.Players
                     .Where(x => Players.Contains(x.PlayerId))
                     .SelectMany(x => x.Villages);
            if (query is null) return playerQuery;
            return query
                .Union(playerQuery);
        }

        private IQueryable<VillageEntity>? GetVillagesFilterQueryable(IQueryable<VillageEntity>? query, List<int> Villages)
        {
            if (Villages.Count == 0) return query;
            var villageQuery = _dbContext.Villages
                .Where(x => Villages.Contains(x.PlayerId));
            if (query is null) return villageQuery;
            return query
                .Union(villageQuery);
        }
    }
}