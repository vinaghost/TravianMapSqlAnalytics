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
            var rawPlayers = await _unitOfRepository.PlayerRepository.GetPlayers(request.Parameters)
                .OrderByDescending(x => x.ChangePopulation)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);

            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. rawPlayers.Select(x => x.AllianceId)], cancellationToken);

            var players = rawPlayers
               .Select(x =>
               {
                   var alliance = alliances[x.AllianceId];
                   return new PlayerContainsPopulationHistoryDto(
                       x.AllianceId,
                       alliance.Name,
                       x.PlayerId,
                       x.PlayerName,
                       x.ChangePopulation,
                       x.Populations.ToList());
               });
            return players;
        }
    }
}