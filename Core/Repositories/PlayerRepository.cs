using Core.Entities;
using Core.Models;
using Core.Parameters;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public class PlayerRepository(ServerDbContext dbContext) : IPlayerRepository
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, PlayerRecord>> GetRecords(List<int> playersId, CancellationToken cancellationToken)
        {
            var ids = playersId.Distinct().Order().ToList();
            return await _dbContext.Players
                .Where(x => ids.Contains(x.PlayerId))
                .ToDictionaryAsync(x => x.PlayerId, x => new PlayerRecord(x.AllianceId, x.Name), cancellationToken: cancellationToken);
        }

        public IEnumerable<PlayerContainsPopulation> GetPlayers(PlayerContainsPopulationParameters parameters)
        {
            return GetBaseQueryable(parameters)
                .Select(x => new PlayerContainsPopulation(
                     x.AllianceId,
                     x.PlayerId,
                     x.Name,
                     x.Villages.Count(),
                     x.Villages.Sum(x => x.Population)
                 ));
        }

        public IEnumerable<PlayerContainsAllianceHistory> GetPlayers(PlayerContainsAllianceHistoryParameters parameters)
        {
            var date = parameters.Date.ToDateTime(TimeOnly.MinValue);
            return GetBaseQueryable(parameters)
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    PlayerName = x.Name,
                    Alliances = x.Alliances
                        .Where(x => x.Date >= date)
                        .OrderByDescending(x => x.Date)
                        .Select(x => new
                        {
                            x.Date,
                            x.AllianceId,
                        })
                })
                .AsEnumerable()
                .Select(x => new PlayerContainsAllianceHistory(

                    x.AllianceId,
                    x.PlayerId,
                    x.PlayerName,
                    x.Alliances.DistinctBy(y => y.AllianceId).Count() - 1,
                    x.Alliances.Select(y => new AllianceHistoryRecord(y.AllianceId, "", y.Date)).ToList()
                ))
                .Where(x => x.ChangeAlliance >= parameters.MinChangeAlliance)
                .Where(x => x.ChangeAlliance <= parameters.MaxChangeAlliance);
        }

        public IEnumerable<PlayerContainsPopulationHistory> GetPlayers(PlayerContainsPopulationHistoryParameters parameters)
        {
            var date = parameters.Date.ToDateTime(TimeOnly.MinValue);

            return GetBaseQueryable(parameters)
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    PlayerName = x.Name,
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
                .AsEnumerable()
                .Select(x => new PlayerContainsPopulationHistory(
                    x.AllianceId,
                    x.PlayerId,
                    x.PlayerName,
                    x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                    x.Populations.Select(x => new PopulationHistoryRecord(x.Population, x.Date)).ToList()
                ))
                .Where(x => x.ChangePopulation >= parameters.MinChangePopulation)
                .Where(x => x.ChangePopulation <= parameters.MaxChangePopulation);
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