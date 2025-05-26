using Features.Constraints;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Features.Queries.Servers
{
    [Handler]
    public static partial class GetServerCountQuery
    {
        public sealed record Query() : DefaultCachedQuery(nameof(GetServerCountQuery), false);

        private static async ValueTask<int> HandleAsync(
            Query query,
            ServerDbContext context,
            CancellationToken cancellationToken
        )
        {
            var count = await context
                .Servers
                .CountAsync(cancellationToken);
            return count;
        }
    }
}