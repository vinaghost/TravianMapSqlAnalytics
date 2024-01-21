using Core.Entities;
using Core.Features.Shared.Dtos;
using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.Shared.Handler
{
    public class VillageQueryHandler(ServerDbContext dbContext)
    {
        protected readonly ServerDbContext _dbContext = dbContext;

        protected async Task<List<int>> GetVillageIds(IVillageFilterParameters parameters, CancellationToken cancellationToken)
        {
            var query = GetBaseQueryable(parameters)
                .Where(x => x.Population >= parameters.MinPopulation)
                .Where(x => x.Population <= parameters.MaxPopulation);
            if (parameters.Tribe != DefaultParameters.Tribe)
            {
                query = query
                    .Where(x => x.Tribe == parameters.Tribe);
            }
            if (parameters.IgnoreCapital)
            {
                query = query
                    .Where(x => x.IsCapital == false);
            }

            if (parameters.IgnoreNormalVillage)
            {
                query = query
                    .Where(x => x.IsCapital == true);
            }
            return await query
                .Select(x => x.VillageId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public IEnumerable<VillageDto> GetVillageDtos(IList<int> villageIds)
        {
            return _dbContext.Villages
                .Where(x => villageIds.Distinct().Contains(x.VillageId))
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

        protected async Task<Dictionary<int, PlayerRecord>> GetPlayers(IList<int> villageIds, CancellationToken cancellationToken)
        {
            return await _dbContext.Players
                .Join(_dbContext.Villages.Where(x => villageIds.Distinct().Contains(x.VillageId)),
                    x => x.PlayerId,
                    x => x.PlayerId,
                    (player, village) => new { player.AllianceId, player.Name, player.PlayerId })
                .Distinct()
                .ToDictionaryAsync(x => x.PlayerId, x => new PlayerRecord(x.AllianceId, x.Name), cancellationToken);
        }

        protected async Task<Dictionary<int, AllianceRecord>> GetAlliances(IList<int> playerIds, CancellationToken cancellationToken)
        {
            return await _dbContext.Alliances
                .Join(_dbContext.Players.Where(x => playerIds.Distinct().Contains(x.PlayerId)),
                    x => x.AllianceId,
                    x => x.AllianceId,
                    (alliance, player) => new { alliance.AllianceId, alliance.Name })
                .Distinct()
                .ToDictionaryAsync(x => x.AllianceId, x => new AllianceRecord(x.Name), cancellationToken);
        }

        private IQueryable<Village> GetBaseQueryable(IVillageFilterParameters parameters)
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