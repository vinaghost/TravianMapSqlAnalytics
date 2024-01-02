using MediatR;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Repositories;
using X.PagedList;

namespace WebAPI.Queries
{
    public record GetVillagesQuery(VillageParameters Parameters) : ICachedQuery<IPagedList<Village>>
    {
        public string CacheKey => $"{nameof(GetVillagesQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetVillagesQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetVillagesQuery, IPagedList<Village>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<Village>> Handle(GetVillagesQuery request, CancellationToken cancellationToken)
        {
            var rawVillages = await _unitOfRepository.VillageRepository.GetVillages(request.Parameters)
                .OrderByDescending(x => x.Population)
                .ThenBy(x => x.Distance)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);

            var players = await _unitOfRepository.PlayerRepository.GetRecords([.. rawVillages.Select(x => x.PlayerId)], cancellationToken);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. players.Values.Select(x => x.AllianceId)], cancellationToken);

            var villages = rawVillages
                .Select(x =>
                {
                    var player = players[x.PlayerId];
                    var alliance = alliances[player.AllianceId];
                    return new Village(
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
                        x.Tribe);
                });

            return villages;
        }
    }
}