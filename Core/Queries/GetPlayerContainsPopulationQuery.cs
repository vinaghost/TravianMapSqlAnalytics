using Core.Dtos;
using Core.Parameters;
using Core.Repositories;
using MediatR;
using X.PagedList;

namespace Core.Queries
{
    public record GetPlayerContainsPopulationQuery(PlayerContainsPopulationParameters Parameters) : ICachedQuery<IPagedList<PlayerContainsPopulationDto>>
    {
        public string CacheKey => $"{nameof(GetPlayerContainsPopulationQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetPlayerContainsPopulationQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetPlayerContainsPopulationQuery, IPagedList<PlayerContainsPopulationDto>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<PlayerContainsPopulationDto>> Handle(GetPlayerContainsPopulationQuery request, CancellationToken cancellationToken)
        {
            var rawPlayers = await _unitOfRepository.PlayerRepository.GetPlayers(request.Parameters)
                            .OrderByDescending(x => x.VillageCount)
                            .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. rawPlayers.Select(x => x.AllianceId)], cancellationToken);

            var players = rawPlayers
                .Select(x =>
                {
                    var alliance = alliances[x.AllianceId];
                    return new PlayerContainsPopulationDto(
                        x.AllianceId,
                        alliance.Name,
                        x.PlayerId,
                        x.PlayerName,
                        x.VillageCount,
                        x.Population);
                });
            return players;
        }
    }
}