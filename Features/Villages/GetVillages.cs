using Features.Shared.Dtos;
using Features.Shared.Enums;
using Features.Shared.Handler;
using Features.Shared.Models;
using Features.Shared.Query;
using Features.Villages.Shared;
using MediatR;
using X.PagedList;

namespace Features.Villages
{
    public record GetVillagesQuery(GetVillagesParameters Parameters) : ICachedQuery<IPagedList<VillageDto>>
    {
        public string CacheKey => $"{nameof(GetVillagesQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetVillagesQueryHandler(VillageDbContext dbContext) : VillageDataQueryHandler(dbContext), IRequestHandler<GetVillagesQuery, IPagedList<VillageDto>>
    {
        public async Task<IPagedList<VillageDto>> Handle(GetVillagesQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;

            var players = GetPlayers(parameters);
            var villages = GetVillages(parameters, parameters);

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

            return await orderDtos.ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
        }
    }
}