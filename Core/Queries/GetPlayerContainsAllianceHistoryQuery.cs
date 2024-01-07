using Core.Models;
using Core.Parameters;
using Core.Repositories;
using MediatR;
using X.PagedList;

namespace Core.Queries
{
    public record GetPlayerContainsAllianceHistoryQuery(PlayerContainsAllianceHistoryParameters Parameters) : ICachedQuery<IPagedList<PlayerContainsAllianceHistoryDetail>>
    {
        public string CacheKey => $"{nameof(GetPlayerContainsAllianceHistoryQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetChangeAlliancePlayersQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetPlayerContainsAllianceHistoryQuery, IPagedList<PlayerContainsAllianceHistoryDetail>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<PlayerContainsAllianceHistoryDetail>> Handle(GetPlayerContainsAllianceHistoryQuery request, CancellationToken cancellationToken)
        {
            var rawPlayers = await _unitOfRepository.PlayerRepository.GetPlayers(request.Parameters)
                .OrderByDescending(x => x.ChangeAlliance)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);

            var oldAllianceId = rawPlayers.SelectMany(x => x.Alliances).DistinctBy(x => x.AllianceId).Select(x => x.AllianceId);
            var currentAllianceId = rawPlayers.Select(x => x.AllianceId);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. currentAllianceId.Concat(oldAllianceId)], cancellationToken);

            var players = rawPlayers
                .Select(x =>
                {
                    var alliance = alliances[x.AllianceId];

                    return new PlayerContainsAllianceHistoryDetail(
                            x.AllianceId,
                            alliance.Name,
                            x.PlayerId,
                            x.PlayerName,
                            x.ChangeAlliance,
                            x.Alliances
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
            return players;
        }
    }
}