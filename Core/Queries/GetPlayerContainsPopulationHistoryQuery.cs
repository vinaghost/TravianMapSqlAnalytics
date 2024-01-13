using Core.Dtos;
using Core.Parameters;
using Core.Repositories;
using MediatR;
using X.PagedList;

namespace Core.Queries
{
    public record GetPlayerContainsPopulationHistoryQuery(PlayerContainsPopulationHistoryParameters Parameters) : ICachedQuery<IPagedList<PlayerContainsPopulationHistoryDto>>
    {
        public string CacheKey => $"{nameof(GetPlayerContainsPopulationHistoryQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetPlayerContainsPopulationHistoryQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetPlayerContainsPopulationHistoryQuery, IPagedList<PlayerContainsPopulationHistoryDto>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<PlayerContainsPopulationHistoryDto>> Handle(GetPlayerContainsPopulationHistoryQuery request, CancellationToken cancellationToken)
        {
            var playerIds = await _unitOfRepository.PlayerRepository.GetPlayerIds(request.Parameters, cancellationToken);
            var players = await _unitOfRepository.PlayerRepository.GetPlayerPopulationHistory(playerIds, request.Parameters, cancellationToken);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. players.Keys], cancellationToken);

            return await _unitOfRepository.PlayerRepository.GetPlayers([.. players.Keys])
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
                })
                .OrderByDescending(x => x.ChangePopulation)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }
    }
}