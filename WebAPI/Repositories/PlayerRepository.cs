using Core;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using PlayerEntity = Core.Models.Player;

namespace WebAPI.Repositories
{
    public class PlayerRepository(ServerDbContext dbContext) : IPlayerRepository
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, RecordPlayer>> GetRecords(List<int> playersId, CancellationToken cancellationToken)
        {
            return await _dbContext.Players
                .Where(x => playersId.Contains(x.PlayerId))
                .ToDictionaryAsync(x => x.PlayerId, x => new RecordPlayer(x.AllianceId, x.Name), cancellationToken: cancellationToken);
        }

        public IQueryable<PlayerEntity> GetQueryable(IPlayerFilterParameter parameters)
        {
            return GetBaseQueryable(parameters);
        }

        private IQueryable<PlayerEntity> GetBaseQueryable(IPlayerFilterParameter parameters)
        {
            if (parameters.Alliances.Count == 0 && parameters.Players.Count == 0) return _dbContext.Players.AsQueryable();

            IQueryable<PlayerEntity>? query = null;

            query = GetAlliancesFilterQueryable(query, parameters.Alliances);
            query = GetPlayersFilterQueryable(query, parameters.Players);
            if (query is null) return _dbContext.Players.AsQueryable();
            return query;
        }

        private IQueryable<PlayerEntity>? GetAlliancesFilterQueryable(IQueryable<PlayerEntity>? query, List<int> Alliances)
        {
            if (Alliances.Count == 0) return query;
            var allianceQuery = _dbContext.Alliances
                 .Where(x => Alliances.Contains(x.AllianceId))
                 .SelectMany(x => x.Players);
            if (query is null) return allianceQuery;
            return query
                .Union(allianceQuery);
        }

        private IQueryable<PlayerEntity>? GetPlayersFilterQueryable(IQueryable<PlayerEntity>? query, List<int> Players)
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