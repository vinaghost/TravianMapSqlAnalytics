using Features.Players;
using Features.Shared.Constraints;
using Features.Shared.Parameters;
using Features.Villages;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using LinqKit;
using X.PagedList;
using X.PagedList.Extensions;

namespace Features.Villages.GetVillages
{
    [Handler]
    public static partial class GetVillagesQuery
    {
        public sealed record Query(GetVillagesParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetVillagesQuery)}_{Parameters.Key()}");

        private static ValueTask<IPagedList<DetailVillageDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var parameters = query.Parameters;

            var playerPredicate = (parameters as IPlayerFilterParameters).GetPredicate();

            var players = context.Players
                .AsExpandable()
                .Where(playerPredicate);

            var villagePredicate = (parameters as IVillageFilterParameters).GetPredicate();
            var distancePredicate = (parameters as IDistanceFilterParameters).GetPredicate();

            var villages = context.Villages
                .AsExpandable()
                .Where(villagePredicate.And(distancePredicate));

            var data = players
               .Join(context.Alliances,
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
                .Select(x => new DetailVillageDto(x.AllianceId,
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

            return ValueTask.FromResult(orderDtos.ToPagedList(parameters.PageNumber, parameters.PageSize));
        }
    }
}