using Features.Shared.Constraints;
using FluentValidation;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using LinqKit;
using X.PagedList;
using X.PagedList.Extensions;

namespace Features.Players.GetPlayers
{
    [Handler]
    public static partial class GetPlayersQuery
    {
        public sealed record Query(GetPlayersParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetPlayersQuery)}_{Parameters.Key()}");

        private static ValueTask<IPagedList<PlayerDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var parameters = query.Parameters;
            var playerPredicate = (parameters as IPlayerFilterParameters).GetPredicate();
            var players = context.Players
                .AsExpandable()
                .Where(playerPredicate)
                .OrderByDescending(x => x.VillageCount)
                .Join(context.Alliances,
                   x => x.AllianceId,
                   x => x.Id,
                   (player, alliance) => new PlayerDto
                   (
                       alliance.Id,
                       alliance.Name,
                       player.Id,
                       player.Name,
                       player.VillageCount,
                       player.Population
                   ))
                .ToPagedList(parameters.PageNumber, parameters.PageSize);
            return ValueTask.FromResult(players);
        }
    }
}