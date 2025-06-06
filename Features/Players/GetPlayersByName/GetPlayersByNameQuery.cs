using Features.Shared.Constraints;
using FluentValidation;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using X.PagedList;
using X.PagedList.Extensions;

namespace Features.Players.GetPlayersByName
{
    [Handler]
    public static partial class GetPlayersByNameQuery
    {
        public sealed record Query(GetPlayersByNameParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetPlayersByNameQuery)}_{Parameters.Key()}");

        private static ValueTask<IPagedList<PlayerDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var parameters = query.Parameters;
            var dbQuery = context.Players
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                dbQuery = dbQuery
                    .Where(x => x.Name.StartsWith(parameters.SearchTerm));
            }

            var data = dbQuery
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
                .OrderBy(x => x.PlayerName)
                .Select(x => new PlayerDto(
                        x.AllianceId,
                        x.AllianceName,
                        x.PlayerId,
                        x.PlayerName,
                        x.VillageCount,
                        x.Population
                ))
                .ToPagedList(parameters.PageNumber, parameters.PageSize);

            return ValueTask.FromResult(data);
        }
    }
}