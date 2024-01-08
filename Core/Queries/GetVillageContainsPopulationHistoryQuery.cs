using Core.Dtos;
using Core.Parameters;
using Core.Repositories;
using MediatR;
using X.PagedList;

namespace Core.Queries
{
    public record GetVillageContainsPopulationHistoryQuery(VillageContainsPopulationHistoryParameters Parameters) : ICachedQuery<IPagedList<VillageContainsPopulationHistoryDto>>
    {
        public string CacheKey => $"{nameof(GetVillageContainsPopulationHistoryQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetVillageContainsPopulationHistoryQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetVillageContainsPopulationHistoryQuery, IPagedList<VillageContainsPopulationHistoryDto>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<VillageContainsPopulationHistoryDto>> Handle(GetVillageContainsPopulationHistoryQuery request, CancellationToken cancellationToken)
        {
            var rawVillages = await _unitOfRepository.VillageRepository.GetVillages(request.Parameters)
                .OrderByDescending(x => x.ChangePopulation)
                .ThenBy(x => x.Distance)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);

            var players = await _unitOfRepository.PlayerRepository.GetRecords([.. rawVillages.Select(x => x.PlayerId)], cancellationToken);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. players.Values.Select(x => x.AllianceId)], cancellationToken);

            var villages = rawVillages
                .Select(x =>
                {
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
                        x.Distance,
                        x.ChangePopulation,
                        x.Populations.ToList());
                });
            return villages;
        }
    }
}