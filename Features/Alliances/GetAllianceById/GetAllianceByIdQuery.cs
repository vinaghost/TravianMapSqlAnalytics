using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Features.Alliances.GetAllianceById
{
    [Handler]
    public sealed partial class GetAllianceByIdQuery(VillageDbContext context)
    {
        public sealed record Query(int AllianceId) : DefaultCachedQuery($"{nameof(GetAllianceByIdQuery)}_{AllianceId}");

        private async ValueTask<AllianceDto?> HandleAsync(
            Query query,
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