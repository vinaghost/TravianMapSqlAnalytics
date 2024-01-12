using Core.Dtos;
using Core.Parameters;
using Core.Repositories;
using MediatR;
using X.PagedList;

namespace Core.Queries
{
    public record GetVillageContainsDistanceQuery(VillageContainsDistanceParameters Parameters) : ICachedQuery<IPagedList<VillageContainsDistanceDto>>
    {
        public string CacheKey => $"{nameof(GetVillageContainsDistanceQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetVillageContainsDistanceQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetVillageContainsDistanceQuery, IPagedList<VillageContainsDistanceDto>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<VillageContainsDistanceDto>> Handle(GetVillageContainsDistanceQuery request, CancellationToken cancellationToken)
        {
            var rawVillages = await _unitOfRepository.VillageRepository.GetVillages(request.Parameters)
                .OrderBy(x => x.Distance)
                .ThenByDescending(x => x.Population)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);

            var players = await _unitOfRepository.PlayerRepository.GetRecords([.. rawVillages.Select(x => x.PlayerId)], cancellationToken);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. players.Values.Select(x => x.AllianceId)], cancellationToken);

            var villages = rawVillages
                .Select(x =>
                {
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
                        x.Distance);
                });

            return villages;
        }
    }
}