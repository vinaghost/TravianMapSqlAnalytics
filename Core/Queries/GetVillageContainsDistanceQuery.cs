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
            var villageIds = await _unitOfRepository.VillageRepository.GetVillageIds(request.Parameters, cancellationToken);
            var villages = await _unitOfRepository.VillageRepository.GetVillages(villageIds, request.Parameters, cancellationToken);
            var players = await _unitOfRepository.PlayerRepository.GetRecords([.. villages.Keys], cancellationToken);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. players.Keys], cancellationToken);

            return await _unitOfRepository.VillageRepository.GetVillages([.. villages.Keys])
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
                })
                .OrderBy(x => x.Distance)
                .ThenByDescending(x => x.Population)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }
    }
}