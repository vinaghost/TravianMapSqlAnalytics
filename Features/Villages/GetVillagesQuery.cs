using Features.Shared.Constraints;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using LinqKit;
using X.PagedList;
using X.PagedList.Extensions;

namespace Features.Villages
{
    [Handler]
    public static partial class GetVillagesQuery
    {
        public sealed record Query(VillagesParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetVillagesQuery)}_{Parameters.Key()}", true);

        private static ValueTask<IPagedList<DetailVillageDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var parameters = query.Parameters;

            var players = context.Players
                .AsExpandable()
                .Where(VillageDataQuery.PlayerPredicate(parameters));

            var villages = context.Villages
                .AsExpandable()
                .Where(VillageDataQuery.VillagePredicate(parameters, parameters));

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