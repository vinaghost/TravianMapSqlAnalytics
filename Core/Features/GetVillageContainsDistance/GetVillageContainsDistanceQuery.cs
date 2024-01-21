using Core.Features.Shared.Handler;
using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Core.Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Core.Features.GetVillageContainsDistance
{
    public record GetVillageContainsDistanceQuery(VillageContainsDistanceParameters Parameters) : ICachedQuery<IPagedList<VillageContainsDistanceDto>>
    {
        public string CacheKey => $"{nameof(GetVillageContainsDistanceQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetVillageContainsDistanceQueryHandler(ServerDbContext dbContext) : VillageQueryHandler(dbContext), IRequestHandler<GetVillageContainsDistanceQuery, IPagedList<VillageContainsDistanceDto>>
    {
        public async Task<IPagedList<VillageContainsDistanceDto>> Handle(GetVillageContainsDistanceQuery request, CancellationToken cancellationToken)
        {
            var villageIds = await GetVillageIds(request.Parameters, cancellationToken);
            var villages = await GetVillages(villageIds, request.Parameters, cancellationToken);
            var players = await GetPlayers([.. villages.Keys], cancellationToken);
            var alliances = await GetAlliances([.. players.Keys], cancellationToken);

            var query = GetVillageDtos([.. villages.Keys])
                .Select(x =>
                {
                    var village = villages[x.VillageId];
                    var player = players[x.PlayerId];
                    var alliance = alliances[player.AllianceId];
                    return new VillageContainsDistanceDto(
                        player.AllianceId,
                        alliance.Name,
                        x.PlayerId,
                        player.Name,
                        x.VillageId,
                        x.VillageName,
                        x.X,
                        x.Y,
                        x.Population,
                        x.IsCapital,
                        x.Tribe,
                        village.Distance);
                });
            var orderedQuery = request.Parameters.SortField.ToLower() switch
            {
                "population" => request.Parameters.SortOrder switch
                {
                    1 => query.OrderByDescending(x => x.Population),
                    _ => query.OrderBy(x => x.Population),
                },

                "distance" => request.Parameters.SortOrder switch
                {
                    1 => query.OrderByDescending(x => x.Distance),
                    _ => query.OrderBy(x => x.Distance),
                },

                _ => query.OrderBy(x => x.Distance)
            };

            return await orderedQuery
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }

        public async Task<Dictionary<int, VillageInfo>> GetVillages(IList<int> villageIds, IDistanceFilterParameters parameters, CancellationToken cancellationToken)
        {
            var centerCoordinate = new Coordinates(parameters.TargetX, parameters.TargetY);
            return await _dbContext.Villages
                .Where(x => villageIds.Distinct().Contains(x.VillageId))
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
    }
}