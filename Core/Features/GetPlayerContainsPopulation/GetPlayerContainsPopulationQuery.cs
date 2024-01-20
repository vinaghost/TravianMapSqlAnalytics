using Core.Features.Shared.Handler;
using Core.Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Core.Features.GetPlayerContainsPopulation
{
    public record GetPlayerContainsPopulationQuery(PlayerContainsPopulationParameters Parameters) : ICachedQuery<IPagedList<PlayerContainsPopulationDto>>
    {
        public string CacheKey => $"{nameof(GetPlayerContainsPopulationQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetPlayerContainsPopulationQueryHandler(ServerDbContext dbContext) : PlayerQueryHandler(dbContext), IRequestHandler<GetPlayerContainsPopulationQuery, IPagedList<PlayerContainsPopulationDto>>
    {
        public async Task<IPagedList<PlayerContainsPopulationDto>> Handle(GetPlayerContainsPopulationQuery request, CancellationToken cancellationToken)
        {
            var playerIds = await GetPlayerIds(request.Parameters, cancellationToken);
            var players = await GetPlayers(playerIds, cancellationToken);
            var alliances = await GetAlliances([.. players.Keys], cancellationToken);

            var query = GetPlayerDtos([.. players.Keys])
                .Select(x =>
                {
                    var alliance = alliances[x.AllianceId];
                    var player = players[x.PlayerId];
                    return new PlayerContainsPopulationDto(
                        x.AllianceId,
                        alliance.Name,
                        x.PlayerId,
                        x.PlayerName,
                        player.VillageCount,
                        player.Population);
                });

            var orderedQuery = request.Parameters.SortField.ToLower() switch
            {
                "villagecount" => request.Parameters.SortOrder switch
                {
                    1 => query.OrderByDescending(x => x.VillageCount),
                    _ => query.OrderBy(x => x.VillageCount),
                },
                "population" => request.Parameters.SortOrder switch
                {
                    1 => query.OrderByDescending(x => x.Population),
                    _ => query.OrderBy(x => x.Population),
                },

                _ => query.OrderByDescending(x => x.VillageCount)
            };

            return await orderedQuery
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }

        private async Task<Dictionary<int, PlayerInfo>> GetPlayers(IList<int> playerIds, CancellationToken cancellationToken)
        {
            return await _dbContext.Players
                .Where(x => playerIds.Distinct().Contains(x.PlayerId))
                .Select(x => new
                {
                    x.PlayerId,
                    VillageCount = x.Villages.Count(),
                    Population = x.Villages.Select(x => x.Population).Sum()
                })
                .ToDictionaryAsync(x => x.PlayerId, x => new PlayerInfo(x.VillageCount, x.Population), cancellationToken);
        }
    }
}