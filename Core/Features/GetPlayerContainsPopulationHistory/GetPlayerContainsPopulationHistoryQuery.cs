using Core.Features.Shared.Handler;
using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Core.Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Core.Features.GetPlayerContainsPopulationHistory
{
    public record GetPlayerContainsPopulationHistoryQuery(PlayerContainsPopulationHistoryParameters Parameters) : ICachedQuery<IPagedList<PlayerContainsPopulationHistoryDto>>
    {
        public string CacheKey => $"{nameof(GetPlayerContainsPopulationHistoryQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetPlayerContainsPopulationHistoryQueryHandler(ServerDbContext dbContext) : PlayerQueryHandler(dbContext), IRequestHandler<GetPlayerContainsPopulationHistoryQuery, IPagedList<PlayerContainsPopulationHistoryDto>>
    {
        public async Task<IPagedList<PlayerContainsPopulationHistoryDto>> Handle(GetPlayerContainsPopulationHistoryQuery request, CancellationToken cancellationToken)
        {
            var playerIds = await GetPlayerIds(request.Parameters, cancellationToken);
            var players = await GetPlayers(playerIds, request.Parameters, cancellationToken);
            var alliances = await GetAlliances([.. players.Keys], cancellationToken);

            var query = GetPlayerDtos([.. players.Keys])
                .Select(x =>
                {
                    var alliance = alliances[x.AllianceId];
                    var player = players[x.PlayerId];
                    return new PlayerContainsPopulationHistoryDto(
                        x.AllianceId,
                        alliance.Name,
                        x.PlayerId,
                        x.PlayerName,
                        player.ChangePopulation,
                        player.Populations);
                });

            var orderedQuery = request.Parameters.SortField.ToLower() switch
            {
                "changepopulation" => request.Parameters.SortOrder switch
                {
                    1 => query.OrderByDescending(x => x.ChangePopulation),
                    _ => query.OrderBy(x => x.ChangePopulation),
                },

                _ => query.OrderBy(x => x.ChangePopulation)
            };
            return await orderedQuery
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }

        private async Task<Dictionary<int, PlayerPopulationHistory>> GetPlayers(IList<int> playerIds, IPopulationHistoryFilterParameters parameters, CancellationToken cancellationToken)
        {
            return await _dbContext.Players
                .Where(x => playerIds.Distinct().Contains(x.PlayerId))
                .Select(x => new
                {
                    x.PlayerId,
                    Populations = x.Villages
                        .SelectMany(x => x.Populations
                                        .Where(x => x.Date >= parameters.Date))
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
    }
}