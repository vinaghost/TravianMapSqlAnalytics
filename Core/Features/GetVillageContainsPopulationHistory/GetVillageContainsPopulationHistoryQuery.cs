using Core.Features.Shared.Handler;
using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Core.Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Core.Features.GetVillageContainsPopulationHistory
{
    public record GetVillageContainsPopulationHistoryQuery(VillageContainsPopulationHistoryParameters Parameters) : ICachedQuery<IPagedList<VillageContainsPopulationHistoryDto>>
    {
        public string CacheKey => $"{nameof(GetVillageContainsPopulationHistoryQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetVillageContainsPopulationHistoryQueryHandler(ServerDbContext dbContext) : VillageQueryHandler(dbContext), IRequestHandler<GetVillageContainsPopulationHistoryQuery, IPagedList<VillageContainsPopulationHistoryDto>>
    {
        public async Task<IPagedList<VillageContainsPopulationHistoryDto>> Handle(GetVillageContainsPopulationHistoryQuery request, CancellationToken cancellationToken)
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
                    return new VillageContainsPopulationHistoryDto(
                        player.AllianceId,
                        alliance.Name,
                        x.PlayerId,
                        player.Name,
                        x.VillageId,
                        x.VillageName,
                        x.X,
                        x.Y,
                        x.IsCapital,
                        x.Tribe,
                        village.ChangePopulation,
                        village.Populations);
                });

            var orderedQuery = request.Parameters.SortField.ToLower() switch
            {
                //"population" => request.Parameters.SortOrder switch
                //{
                //    1 => query.OrderByDescending(x => x.Population),
                //    _ => query.OrderBy(x => x.Population),
                //},

                "ChangePopulation" => request.Parameters.SortOrder switch
                {
                    1 => query.OrderByDescending(x => x.ChangePopulation),
                    _ => query.OrderBy(x => x.ChangePopulation),
                },

                _ => query.OrderBy(x => x.ChangePopulation)
            };
            return await orderedQuery
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }

        public async Task<Dictionary<int, VillagePopulationHistory>> GetVillages(IList<int> villageIds, IPopulationHistoryFilterParameters parameters, CancellationToken cancellationToken)
        {
            return await _dbContext.VillagesPopulations
                .Where(x => villageIds.Distinct().Contains(x.VillageId))
                .Where(x => x.Date >= parameters.Date)
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
                .ToDictionaryAsync(x => x.VillageId, x => new VillagePopulationHistory(x.ChangePopulation, x.Populations), cancellationToken);
        }
    }
}