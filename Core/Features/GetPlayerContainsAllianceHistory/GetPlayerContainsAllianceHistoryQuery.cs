using Core.Features.Shared.Handler;
using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Core.Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Core.Features.GetPlayerContainsAllianceHistory
{
    public record GetPlayerContainsAllianceHistoryQuery(PlayerContainsAllianceHistoryParameters Parameters) : ICachedQuery<IPagedList<PlayerContainsAllianceHistoryDto>>
    {
        public string CacheKey => $"{nameof(GetPlayerContainsAllianceHistoryQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetChangeAlliancePlayersQueryHandler(ServerDbContext dbContext) : PlayerQueryHandler(dbContext), IRequestHandler<GetPlayerContainsAllianceHistoryQuery, IPagedList<PlayerContainsAllianceHistoryDto>>
    {
        public async Task<IPagedList<PlayerContainsAllianceHistoryDto>> Handle(GetPlayerContainsAllianceHistoryQuery request, CancellationToken cancellationToken)
        {
            var playerIds = await GetPlayerIds(request.Parameters, cancellationToken);
            var players = await GetPlayers(playerIds, request.Parameters, cancellationToken);
            var alliances = await GetAlliances([.. players.Keys], request.Parameters, cancellationToken);

            var query = GetPlayerDtos([.. players.Keys])
                .Select(x =>
                {
                    var alliance = alliances[x.AllianceId];
                    var player = players[x.PlayerId];
                    return new PlayerContainsAllianceHistoryDto(
                            x.AllianceId,
                            alliance.Name,
                            x.PlayerId,
                            x.PlayerName,
                            player.ChangeAlliance,
                            player.Alliances
                                .Select(ally =>
                                {
                                    var allianceHistory = alliances[ally.AllianceId];
                                    return new AllianceHistoryRecord(
                                        ally.AllianceId,
                                        allianceHistory.Name,
                                        ally.Date);
                                })
                                .ToList());
                });

            var orderedQuery = request.Parameters.SortField.ToLower() switch
            {
                "changealliance" => request.Parameters.SortOrder switch
                {
                    1 => query.OrderByDescending(x => x.ChangeAlliance),
                    _ => query.OrderBy(x => x.ChangeAlliance),
                },

                _ => query.OrderByDescending(x => x.ChangeAlliance)
            };

            return await orderedQuery
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }

        public async Task<Dictionary<int, PlayerAllianceHistory>> GetPlayers(IList<int> playerIds, IAllianceHistoryFilterParameters parameters, CancellationToken cancellationToken)
        {
            return await _dbContext.Players
                .Where(x => playerIds.Distinct().Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    Alliances = x.Alliances
                        .Where(x => x.Date >= parameters.Date)
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
                    x.Id,
                    ChangeAlliance = x.Alliances.DistinctBy(y => y.AllianceId).Count() - 1,
                    Alliances = x.Alliances.Select(y => new AllianceHistoryRecord(y.AllianceId, "", y.Date)).ToList()
                })
                .Where(x => x.ChangeAlliance >= parameters.MinChangeAlliance)
                .Where(x => x.ChangeAlliance <= parameters.MaxChangeAlliance)
                .ToDictionaryAsync(x => x.Id, x => new PlayerAllianceHistory(x.ChangeAlliance, x.Alliances), cancellationToken);
        }

        public async Task<Dictionary<int, AllianceRecord>> GetAlliances(IList<int> playerIds, IHistoryParameters parameters, CancellationToken cancellationToken)
        {
            var allianceCurrentIds = _dbContext.Players
                .Where(x => playerIds.Distinct().Contains(x.Id))
                .Select(x => x.AllianceId);

            var allianceHistoryIds = _dbContext.PlayerAllianceHistory
                .Where(x => playerIds.Distinct().Contains(x.PlayerId))
                .Where(x => x.Date >= parameters.Date)
                .Select(x => x.AllianceId);

            var allianceIds = allianceCurrentIds
                .Union(allianceHistoryIds)
                .Distinct();

            return await _dbContext.Alliances
                .Where(x => allianceIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => new AllianceRecord(x.Name), cancellationToken);
        }
    }
}