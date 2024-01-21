using Core.Entities;
using Core.Features.Shared.Dtos;
using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.Shared.Handler
{
    public class PlayerQueryHandler(ServerDbContext dbContext)
    {
        protected readonly ServerDbContext _dbContext = dbContext;

        protected async Task<List<int>> GetPlayerIds(IPlayerFilterParameters parameters, CancellationToken cancellationToken)
        {
            return await GetBaseQueryable(parameters)
                .Select(x => x.PlayerId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        protected IEnumerable<PlayerDto> GetPlayerDtos(IList<int> playerIds)
        {
            return _dbContext.Players
                .Where(x => playerIds.Distinct().Contains(x.PlayerId))
                .Select(x => new PlayerDto(
                    x.PlayerId,
                    x.Name,
                    x.AllianceId
                    ))
                .AsEnumerable();
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

        private IQueryable<Player> GetBaseQueryable(IPlayerFilterParameters parameters)
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