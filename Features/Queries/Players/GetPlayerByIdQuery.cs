using Features.Constraints;
using Features.Dtos;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Features.Queries.Players
{
    [Handler]
    public static partial class GetPlayerByIdQuery
    {
        public sealed record Query(int PlayerId) : DefaultCachedQuery($"{nameof(GetPlayerByIdQuery)}_{PlayerId}");

        private static async ValueTask<PlayerDto?> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var playerId = query.PlayerId;
            var player = await context.Players
                .Where(x => x.Id == playerId)
                .Join(context.Alliances,
                    x => x.AllianceId,
                    x => x.Id,
                    (player, alliance) => new PlayerDto(
                        alliance.Id,
                        alliance.Name,
                        player.Id,
                        player.Name,
                        player.VillageCount,
                        player.Population
                    ))
                .FirstOrDefaultAsync(cancellationToken);
            return player;
        }
    }
}