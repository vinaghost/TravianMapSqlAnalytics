using Features.Shared.Dtos;
using Features.Shared.Enums;
using Features.Shared.Handler;
using Features.Shared.Models;
using Features.Shared.Query;
using Features.Villages.Shared;

namespace Features.Villages
{
    public record GetVillagesByParameters(VillagesParameters Parameters) : ICachedQuery<IPagedList<VillageDto>>
    {
        public string CacheKey => $"{nameof(GetVillagesByParameters)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetVillagesByParametersQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetVillagesByParameters, IPagedList<VillageDto>>
    {
        private readonly VillageDbContext _dbContext = dbContext;

        public async Task<IPagedList<VillageDto>> Handle(GetVillagesByParameters request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;

            var players = _dbContext.Players
                .AsExpandable()
                .Where(VillageDataQuery.PlayerPredicate(parameters));

            var villages = _dbContext.Villages
                .AsExpandable()
                .Where(VillageDataQuery.VillagePredicate(parameters, parameters));

            var data = players
               .Join(_dbContext.Alliances,
                   x => x.AllianceId,
                   x => x.Id,
                   (player, alliance) => new
                   {
                       PlayerId = player.Id,
                       PlayerName = player.Name,
                       AllianceId = alliance.Id,
                       AllianceName = alliance.Name,
                       player.Population,
                       player.VillageCount
                   })
                .Join(villages,
                    x => x.PlayerId,
                    x => x.PlayerId,
                    (player, village) => new
                    {
                        player.PlayerId,
                        player.PlayerName,
                        player.AllianceId,
                        player.AllianceName,
                        VillageId = village.Id,
                        village.MapId,
                        village.Name,
                        village.X,
                        village.Y,
                        village.Population,
                        village.Tribe,
                        village.IsCapital,
                    })
                .AsEnumerable();

            var centerCoordinate = new Coordinates(parameters.X, parameters.Y);

            var dtos = data
                .Select(x => new VillageDto(x.AllianceId,
                                            x.AllianceName,
                                            x.PlayerId,
                                            x.PlayerName,
                                            x.VillageId,
                                            x.MapId,
                                            x.Name,
                                            x.X,
                                            x.Y,
                                            x.IsCapital,
                                            (Tribe)x.Tribe,
                                            x.Population,
                                            centerCoordinate.Distance(x.X, x.Y)));

            var orderDtos = dtos
                .OrderBy(x => x.Distance);

            return orderDtos.ToPagedList(parameters.PageNumber, parameters.PageSize);
        }
    }
}