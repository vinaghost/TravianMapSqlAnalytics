using Features.Constraints;
using Features.Queries.Villages.Shared;
using Features.Shared.Dtos;
using Features.Shared.Enums;
using Features.Shared.Handler;
using Features.Shared.Models;
using Immediate.Handlers.Shared;

namespace Features.Queries.Villages
{
    public record GetInactiveVillagesParameters : VillagesParameters
    {
        public int InactiveDays { get; init; } = 3;
    }

    public static class GetInactiveVillagesParametersExtension
    {
        public static string Key(this GetInactiveVillagesParameters parameters)
        {
            return $"{parameters.KeyParent()}_{parameters.InactiveDays}";
        }
    }

    public class GetInactiveVillagesParametersValidator : AbstractValidator<GetInactiveVillagesParameters>
    {
        public GetInactiveVillagesParametersValidator()
        {
            Include(new VillagesParametersValidator());

            RuleFor(x => x.InactiveDays)
                .NotEmpty()
                .GreaterThanOrEqualTo(3);
        }
    }

    [Handler]
    public static partial class GetInactiveVillagesQuery
    {
        public sealed record Query(GetInactiveVillagesParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetInactiveVillagesQuery)}_{Parameters.Key()}", true);

        private static ValueTask<IPagedList<VillageDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var parameters = query.Parameters;

            var players = GetInactivePlayers(parameters, context);
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

            return ValueTask.FromResult(orderDtos.ToPagedList(parameters.PageNumber, parameters.PageSize));
        }

        private static IQueryable<int> GetInactivePlayerIds(GetInactiveVillagesParameters parameters, VillageDbContext context)
        {
            var date = DateTime.Today.AddDays(-parameters.InactiveDays);
            if (VillageDataQuery.IsPlayerFiltered(parameters))
            {
                var query = context.Villages
                    .AsExpandable()
                    .Where(VillageDataQuery.VillagePredicate(parameters, parameters))
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

        private static IQueryable<Player> GetInactivePlayers(GetInactiveVillagesParameters parameters, VillageDbContext context)
        {
            var ids = GetInactivePlayerIds(parameters, context);
            return context.Players
                .Where(x => ids.Contains(x.Id));
        }
    }
}