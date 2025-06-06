using Features.Shared.Constraints;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Features.Queries.Servers
{
    [Handler]
    public static partial class GetMostPlayerServerQuery
    {
        public sealed record Query() : DefaultCachedQuery(nameof(GetMostPlayerServerQuery), false);

        private static async ValueTask<string> HandleAsync(
            Query query,
            ServerDbContext context,
            CancellationToken cancellationToken
        )
        {
            var server = await context.Servers
                .OrderByDescending(x => x.PlayerCount)
                .Select(x => x.Url)
                .FirstOrDefaultAsync(cancellationToken);
            return server ?? "";
        }
    }
}