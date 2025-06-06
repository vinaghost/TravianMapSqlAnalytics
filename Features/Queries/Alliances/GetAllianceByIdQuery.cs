using Features.Dtos;
using Features.Shared.Constraints;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Features.Queries.Alliances
{
    [Handler]
    public static partial class GetAllianceByIdQuery
    {
        public sealed record Query(int AllianceId) : DefaultCachedQuery($"{nameof(GetAllianceByIdQuery)}_{AllianceId}");

        private static async ValueTask<AllianceDto?> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var allianceId = query.AllianceId;
            var alliance = await context.Alliances
                .Where(x => x.Id == allianceId)
                .Select(x => new AllianceDto(
                    x.Id,
                    string.IsNullOrEmpty(x.Name) ? "No alliance" : x.Name,
                    x.PlayerCount
                ))
                .FirstOrDefaultAsync(cancellationToken);
            return alliance;
        }
    }
}