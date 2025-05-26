using Features.Constraints;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Features.Queries.Servers
{
    [Handler]
    public static partial class IsValidServerUrlQuery
    {
        public sealed record Query(string ServerUrl) : DefaultCachedQuery($"{nameof(IsValidServerUrlQuery)}_{ServerUrl}", false);

        private static async ValueTask<bool> HandleAsync(
            Query query,
            ServerDbContext context,
            CancellationToken cancellationToken
        )
        {
            return await context.Servers
                .Where(x => x.Url == query.ServerUrl)
                .AnyAsync(cancellationToken);
        }
    }
}