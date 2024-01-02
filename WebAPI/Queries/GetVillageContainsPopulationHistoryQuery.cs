using MediatR;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Repositories;
using X.PagedList;

namespace WebAPI.Queries
{
    public record GetChangePopulationVillagesQuery(VillageContainsPopulationHistoryParameters Parameters) : ICachedQuery<IPagedList<VillageHasChangePopulation>>
    {
        public string CacheKey => $"{nameof(GetChangePopulationVillagesQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetChangePopulationVillagesQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetChangePopulationVillagesQuery, IPagedList<VillageHasChangePopulation>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<VillageHasChangePopulation>> Handle(GetChangePopulationVillagesQuery request, CancellationToken cancellationToken)
        {
            var rawVillageQueryable = _unitOfRepository.VillageRepository.GetQueryable(request.Parameters);

            var date = request.Parameters.Date.ToDateTime(TimeOnly.MinValue);

            var rawVillages = await rawVillageQueryable
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    Populations = x.Populations.OrderByDescending(x => x.Date).Where(x => x.Date >= date),
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    VillageName = x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    ChangePopulation = x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                    Populations = x.Populations.Select(x => new PopulationHistoryRecord(x.Population, x.Date))
                })
                .Where(x => x.ChangePopulation >= request.Parameters.MinChangePopulation)
                .Where(x => x.ChangePopulation <= request.Parameters.MaxChangePopulation)
                .OrderByDescending(x => x.ChangePopulation)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);

            var players = await _unitOfRepository.PlayerRepository.GetRecords([.. rawVillages.Select(x => x.PlayerId)], cancellationToken);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. players.Values.Select(x => x.AllianceId)], cancellationToken);

            var villages = rawVillages
                .Select(x =>
                {
                    var player = players[x.PlayerId];
                    var alliance = alliances[player.AllianceId];
                    return new VillageHasChangePopulation(
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
                        x.ChangePopulation,
                        x.Populations);
                });
            return villages;
        }
    }
}