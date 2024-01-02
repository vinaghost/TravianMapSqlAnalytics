using Core.Models;
using Core.Parameters;
using Microsoft.EntityFrameworkCore;
using Player = Core.Entities.Player;

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

        public IQueryable<Player> GetQueryable(IPlayerFilterParameter parameters)
        {
            return GetBaseQueryable(parameters);
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