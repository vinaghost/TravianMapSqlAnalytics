using Features.Constraints;
using Features.Dtos;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Features.Queries.Servers
{
    [Handler]
    public static partial class GetServerByIdQuery
    {
        public sealed record Query(int ServerId) : DefaultCachedQuery($"{nameof(GetServerByIdQuery)}_{ServerId}", false);

        private static async ValueTask<ServerDto?> HandleAsync(
            Query query,
            ServerDbContext context,
            CancellationToken cancellationToken
        )
        {
            return await context.Servers
                .Where(x => x.Id == query.ServerId)
                .Select(x => new ServerDto(x.Id, x.Url))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}