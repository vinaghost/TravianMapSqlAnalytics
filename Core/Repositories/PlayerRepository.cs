using Core.Dtos;
using Core.Entities;
using Core.Models;
using Core.Parameters;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public class PlayerRepository(ServerDbContext dbContext) : IPlayerRepository
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, PlayerRecord>> GetRecords(IList<int> villageIds, CancellationToken cancellationToken)
        {
            var ids = villageIds.Distinct().Order().ToList();

            var playersId = await _dbContext.Villages
                .Where(x => ids.Contains(x.VillageId))
                .Select(x => x.PlayerId)
                .Distinct()
                .Order()
                .ToListAsync(cancellationToken);

            return await _dbContext.Players
                .Where(x => playersId.Contains(x.PlayerId))
                .ToDictionaryAsync(x => x.PlayerId, x => new PlayerRecord(x.AllianceId, x.Name), cancellationToken);
        }

        public async Task<List<int>> GetPlayerIds(IPlayerFilterParameter parameters, CancellationToken cancellationToken)
        {
            return await GetBaseQueryable(parameters)
                .Select(x => x.PlayerId)
                .Distinct()
                .Order()
                .ToListAsync(cancellationToken);
        }

        public IEnumerable<PlayerDto> GetPlayers(IList<int> playerIds)
        {
            var ids = playerIds.Distinct().Order().ToList();
            return _dbContext.Players
                .Where(x => ids.Contains(x.PlayerId))
                .Select(x => new PlayerDto(
                    x.PlayerId,
                    x.Name,
                    x.AllianceId
                    ))
                .AsEnumerable();
        }

        public async Task<Dictionary<int, PlayerAllianceHistory>> GetPlayerAllianceHistory(IList<int> playerIds, PlayerContainsAllianceHistoryParameters parameters, CancellationToken cancellationToken)
        {
            var ids = playerIds.Distinct().Order().ToList();
            var date = parameters.Date.ToDateTime(TimeOnly.MinValue);
            return await _dbContext.Players
                .Where(x => ids.Contains(x.PlayerId))
                .Select(x => new
                {
                    x.PlayerId,
                    Alliances = x.Alliances
                        .Where(x => x.Date >= date)
                        .OrderByDescending(x => x.Date)
                        .Select(x => new
                        {
                            x.Date,
                            x.AllianceId,
                        })
                })
                .AsAsyncEnumerable()
                .Select(x => new
                {
                    x.PlayerId,
                    ChangeAlliance = x.Alliances.DistinctBy(y => y.AllianceId).Count() - 1,
                    Alliances = x.Alliances.Select(y => new AllianceHistoryRecord(y.AllianceId, "", y.Date)).ToList()
                })
                .Where(x => x.ChangeAlliance >= parameters.MinChangeAlliance)
                .Where(x => x.ChangeAlliance <= parameters.MaxChangeAlliance)
                .ToDictionaryAsync(x => x.PlayerId, x => new PlayerAllianceHistory(x.ChangeAlliance, x.Alliances), cancellationToken);
        }

        public async Task<Dictionary<int, PlayerPopulationHistory>> GetPlayerPopulationHistory(IList<int> playerIds, PlayerContainsPopulationHistoryParameters parameters, CancellationToken cancellationToken)
        {
            var ids = playerIds.Distinct().Order().ToList();
            var date = parameters.Date.ToDateTime(TimeOnly.MinValue);
            return await _dbContext.Players
                .Where(x => ids.Contains(x.PlayerId))
                .Select(x => new
                {
                    x.PlayerId,
                    Populations = x.Villages
                        .SelectMany(x => x.Populations
                                        .Where(x => x.Date >= date))
                        .GroupBy(x => x.Date)
                        .OrderByDescending(x => x.Key)
                        .Select(x => new
                        {
                            Date = x.Key,
                            Population = x
                                    .OrderBy(x => x.Date)
                                    .Select(x => x.Population)
                                    .Sum(),
                        })
                })
                .AsAsyncEnumerable()
                .Select(x => new
                {
                    x.PlayerId,
                    ChangePopulation = x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                    Populations = x.Populations.Select(x => new PopulationHistoryRecord(x.Population, x.Date)).ToList()
                })
                .Where(x => x.ChangePopulation >= parameters.MinChangePopulation)
                .Where(x => x.ChangePopulation <= parameters.MaxChangePopulation)
                .ToDictionaryAsync(x => x.PlayerId, x => new PlayerPopulationHistory(x.ChangePopulation, x.Populations), cancellationToken);
        }

        public async Task<Dictionary<int, PlayerInfo>> GetPlayerInfo(IList<int> playerIds, CancellationToken cancellationToken)
        {
            var ids = playerIds.Distinct().Order().ToList();
            return await _dbContext.Players
                .Where(x => ids.Contains(x.PlayerId))
                .Select(x => new
                {
                    x.PlayerId,
                    VillageCount = x.Villages.Count(),
                    Population = x.Villages.Select(x => x.Population).Sum()
                })
                .ToDictionaryAsync(x => x.PlayerId, x => new PlayerInfo(x.VillageCount, x.Population), cancellationToken);
        }

        private IQueryable<Player> GetBaseQueryable(IPlayerFilterParameter parameters)
        {
            if (parameters.Alliances.Count == 0 && parameters.Players.Count == 0) return _dbContext.Players.AsQueryable();

            IQueryable<Player> query = null;

            query = GetAlliancesFilterQueryable(query, parameters.Alliances);
            query = GetPlayersFilterQueryable(query, parameters.Players);
            if (query is null) return _dbContext.Players.AsQueryable();
            return query;
        }

        private IQueryable<Player> GetAlliancesFilterQueryable(IQueryable<Player> query, List<int> Alliances)
        {
            if (Alliances.Count == 0) return query;
            var allianceQuery = _dbContext.Alliances
                 .Where(x => Alliances.Contains(x.AllianceId))
                 .SelectMany(x => x.Players);
            if (query is null) return allianceQuery;
            return query
                .Union(allianceQuery);
        }

        private IQueryable<Player> GetPlayersFilterQueryable(IQueryable<Player> query, List<int> Players)
        {
            if (Players.Count == 0) return query;
            var playerQuery = _dbContext.Players
                     .Where(x => Players.Contains(x.PlayerId));
            if (query is null) return playerQuery;
            return query
                .Union(playerQuery);
        }
    }
}