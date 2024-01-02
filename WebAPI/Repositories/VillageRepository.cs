﻿using Core;
using Core.Models;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;

namespace WebAPI.Repositories
{
    public class VillageRepository(ServerDbContext dbContext) : IVillageRepository
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public IEnumerable<VillageContainsDistance> GetVillages(VillageParameters parameters)
        {
            var centerCoordinate = new Coordinates(parameters.TargetX, parameters.TargetY);
            return GetBaseQueryable(parameters)
                .Where(x => x.Population >= parameters.MinPopulation)
                .Where(x => x.Population <= parameters.MaxPopulation)
                .AsEnumerable()
                .Select(x => new VillageContainsDistance(
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.Population,
                    x.IsCapital,
                    x.Tribe,
                    centerCoordinate.Distance(new Coordinates(x.X, x.Y))))
                .Where(x => x.Distance >= parameters.MinDistance)
                .Where(x => x.Distance <= parameters.MaxDistance);
        }

        public IEnumerable<VillageContainPopulationHistory> GetVillages(VillageContainsPopulationHistoryParameters parameters)
        {
            var centerCoordinate = new Coordinates(parameters.TargetX, parameters.TargetY);
            var date = parameters.Date.ToDateTime(TimeOnly.MinValue);
            return GetBaseQueryable(parameters)
                .Where(x => x.Population >= parameters.MinPopulation)
                .Where(x => x.Population <= parameters.MaxPopulation)
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    Populations = x.Populations.OrderByDescending(x => x.Date).Where(x => x.Date >= date),
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    x.Populations,
                    Distance = centerCoordinate.Distance(new Coordinates(x.X, x.Y))
                })
                .Where(x => x.Distance >= parameters.MinDistance)
                .Where(x => x.Distance <= parameters.MaxDistance)
                .Select(x => new VillageContainPopulationHistory(
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    x.Distance,
                    x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                    x.Populations.Select(x => new PopulationHistoryRecord(x.Population, x.Date))))
                .Where(x => x.ChangePopulation >= parameters.MinChangePopulation)
                .Where(x => x.ChangePopulation <= parameters.MaxChangePopulation);
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