using Features.Players;
using Features.Shared.Constraints;
using Features.Shared.Parameters;
using FluentValidation;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using Infrastructure.Entities;
using LinqKit;
using X.PagedList;
using X.PagedList.Extensions;

namespace Features.Villages.GetInactiveVillages
{
    [Handler]
    public static partial class GetInactiveVillagesQuery
    {
        public sealed record Query(GetInactiveVillagesParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetInactiveVillagesQuery)}_{Parameters.Key()}");

        private static ValueTask<IPagedList<DetailVillageDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var parameters = query.Parameters;

            var players = context.GetInactivePlayers(parameters);

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

        private static IQueryable<int> GetInactivePlayerIds(this VillageDbContext context, GetInactiveVillagesParameters parameters)
        {
            var date = DateTime.Today.AddDays(-parameters.InactiveDays);
            if (parameters.IsPlayerFiltered())
            {
                var villagePredicate = (parameters as IVillageFilterParameters).GetPredicate();
                var distancePredicate = (parameters as IDistanceFilterParameters).GetPredicate();
                var query = context.Villages
                    .AsExpandable()
                    .Where(villagePredicate.And(distancePredicate))
                    .Join(context.PlayersHistory
                            .Where(x => x.Date >= date),
                        x => x.Id,
                        x => x.PlayerId,
                        (player, population) => new
                        {
                            player.Id,
                            population.ChangePopulation
                        })
                    .GroupBy(x => x.Id)
                    .Where(x => x.Count() >= parameters.InactiveDays && x.Select(x => x.ChangePopulation).Max() == 0 && x.Select(x => x.ChangePopulation).Min() == 0)
                    .Select(x => x.Key);
                return query;
            }
            else
            {
                var query = context.PlayersHistory
                   .Where(x => x.Date >= date)
                   .GroupBy(x => x.PlayerId)
                   .Where(x => x.Count() >= parameters.InactiveDays && x.Select(x => x.ChangePopulation).Max() == 0 && x.Select(x => x.ChangePopulation).Min() == 0)
                   .Select(x => x.Key);
                return query;
            }
        }

        private static IQueryable<Player> GetInactivePlayers(this VillageDbContext context, GetInactiveVillagesParameters parameters)
        {
            var ids = context.GetInactivePlayerIds(parameters);
            return context.Players
                .Where(x => ids.Contains(x.Id));
        }
    }
}