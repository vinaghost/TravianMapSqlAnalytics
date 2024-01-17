using Core.Dtos;
using Core.Entities;
using Core.Models;
using Core.Parameters;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public class VillageRepository(ServerDbContext dbContext) : IVillageRepository
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public IEnumerable<VillageDto> GetVillages(IList<int> villageIds)
        {
            var ids = villageIds.Distinct().Order().ToList();
            return _dbContext.Villages
                .Where(x => ids.Contains(x.VillageId))
                .Select(x => new VillageDto(
                    x.PlayerId,
                    x.VillageId,
                    x.MapId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.Population,
                    x.IsCapital,
                    x.Tribe))
                .AsEnumerable();
        }

        public async Task<Dictionary<int, VillageInfo>> GetVillages(IList<int> villageIds, VillageContainsDistanceParameters parameters, CancellationToken cancellationToken)
        {
            var ids = villageIds.Distinct().Order().ToList();
            var centerCoordinate = new Coordinates(parameters.TargetX, parameters.TargetY);
            return await _dbContext.Villages
                .Where(x => ids.Contains(x.VillageId))
                .AsAsyncEnumerable()
                .Select(x => new
                {
                    x.VillageId,
                    Distance = centerCoordinate.Distance(new Coordinates(x.X, x.Y))
                })
                .Where(x => x.Distance >= parameters.MinDistance)
                .Where(x => x.Distance <= parameters.MaxDistance)
                .ToDictionaryAsync(x => x.VillageId, x => new VillageInfo(x.Distance), cancellationToken);
        }

        public async Task<Dictionary<int, VillagePopulationHistory>> GetVillages(IList<int> villageIds, VillageContainsPopulationHistoryParameters parameters, CancellationToken cancellationToken)
        {
            var ids = villageIds.Distinct().Order().ToList();
            var centerCoordinate = new Coordinates(parameters.TargetX, parameters.TargetY);
            var date = parameters.Date.ToDateTime(TimeOnly.MinValue);

            var distanceVillages = await _dbContext.Villages
                .Where(x => ids.Contains(x.VillageId))
                .Select(x => new
                {
                    x.VillageId,
                    x.X,
                    x.Y,
                })
                .AsAsyncEnumerable()
                .Select(x => new
                {
                    x.VillageId,
                    Distance = centerCoordinate.Distance(new Coordinates(x.X, x.Y))
                })
                .Where(x => x.Distance >= parameters.MinDistance)
                .Where(x => x.Distance <= parameters.MaxDistance)
                .ToDictionaryAsync(x => x.VillageId, x => x.Distance, cancellationToken);

            ids = [.. distanceVillages.Keys];

            return await _dbContext.VillagesPopulations
                .Where(x => ids.Contains(x.VillageId))
                .Where(x => x.Date >= date)
                .GroupBy(x => x.VillageId)
                .Select(x => new
                {
                    VillageId = x.Key,
                    Populations = x.OrderByDescending(x => x.Date).Select(x => new PopulationHistoryRecord(x.Population, x.Date))
                })
                .AsAsyncEnumerable()
                .Select(x => new
                {
                    x.VillageId,
                    ChangePopulation = x.Populations.Select(x => x.Amount).FirstOrDefault() - x.Populations.Select(x => x.Amount).LastOrDefault(),
                    Populations = x.Populations.ToList()
                })
                .Where(x => x.ChangePopulation >= parameters.MinChangePopulation)
                .Where(x => x.ChangePopulation <= parameters.MaxChangePopulation)
                .ToDictionaryAsync(x => x.VillageId, x => new VillagePopulationHistory(distanceVillages[x.VillageId], x.ChangePopulation, x.Populations), cancellationToken);
        }

        public async Task<List<int>> GetVillageIds(IVillageFilterParameter parameters, CancellationToken cancellationToken)
        {
            return await GetBaseQueryable(parameters)
                .Where(x => x.Population >= parameters.MinPopulation)
                .Where(x => x.Population <= parameters.MaxPopulation)
                .Select(x => x.VillageId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        private IQueryable<Village> GetBaseQueryable(IVillageFilterParameter parameters)
        {
            if (parameters.Alliances.Count == 0 && parameters.Players.Count == 0) return _dbContext.Villages.AsQueryable();

            IQueryable<Village> query = null;

            query = GetAlliancesFilterQueryable(query, parameters.Alliances);
            query = GetPlayersFilterQueryable(query, parameters.Players);

            if (query is null) return _dbContext.Villages.AsQueryable();
            return query;
        }

        private IQueryable<Village> GetAlliancesFilterQueryable(IQueryable<Village> query, List<int> Alliances)
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

        private IQueryable<Village> GetPlayersFilterQueryable(IQueryable<Village> query, List<int> Players)
        {
            if (Players.Count == 0) return query;
            var playerQuery = _dbContext.Players
                     .Where(x => Players.Contains(x.PlayerId))
                     .SelectMany(x => x.Villages);
            if (query is null) return playerQuery;
            return query
                .Union(playerQuery);
        }
    }
}