using Core.Dtos;
using Core.Models;
using Core.Parameters;
using Core.Repositories;
using MediatR;
using X.PagedList;

namespace Core.Queries
{
    public record GetPlayerContainsAllianceHistoryQuery(PlayerContainsAllianceHistoryParameters Parameters) : ICachedQuery<IPagedList<PlayerContainsAllianceHistoryDto>>
    {
        public string CacheKey => $"{nameof(GetPlayerContainsAllianceHistoryQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetChangeAlliancePlayersQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetPlayerContainsAllianceHistoryQuery, IPagedList<PlayerContainsAllianceHistoryDto>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<PlayerContainsAllianceHistoryDto>> Handle(GetPlayerContainsAllianceHistoryQuery request, CancellationToken cancellationToken)
        {
            var playerIds = await _unitOfRepository.PlayerRepository.GetPlayerIds(request.Parameters, cancellationToken);
            var players = await _unitOfRepository.PlayerRepository.GetPlayerAllianceHistory(playerIds, request.Parameters, cancellationToken);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. players.Keys], request.Parameters.Date, cancellationToken);

            return await _unitOfRepository.PlayerRepository.GetPlayers([.. players.Keys])
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
                })
                .OrderBy(x => x.ChangeAlliance)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }
    }
}