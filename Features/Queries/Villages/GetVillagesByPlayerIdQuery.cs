using Features.Constraints;
using Features.Shared.Enums;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Features.Queries.Villages.ByPlayerId
{
    public record VillageDto(int MapId,
                             int VillageId,
                             string VillageName,
                             int X,
                             int Y,
                             Tribe Tribe,
                             int Population,
                             bool IsCapital);

    [Handler]
    public static partial class GetVillagesByPlayerIdQuery
    {
        public sealed record Query(int PlayerId) : DefaultCachedQuery($"{nameof(GetVillagesByPlayerIdQuery)}_{PlayerId}", true);

        private static async ValueTask<IList<VillageDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var playerId = query.PlayerId;
            var villages = await context.Villages
                .Where(x => x.PlayerId == playerId)
                .Select(x => new VillageDto(
                    x.MapId,
                    x.Id,
                    x.Name,
                    x.X,
                    x.Y,
                    (Tribe)x.Tribe,
                    x.Population,
                    x.IsCapital
                ))
                .ToListAsync(cancellationToken);
            return villages;
        }
    }
}