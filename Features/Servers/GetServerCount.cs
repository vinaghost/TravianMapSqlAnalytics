using Features.Shared.Query;
using Microsoft.EntityFrameworkCore;

namespace Features.Servers
{
    public record GetServerCountQuery : ICachedQuery<int>
    {
        public string CacheKey => nameof(GetServerCountQuery);

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServerCountQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetServerCountQuery, int>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<int> Handle(GetServerCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _dbContext
                .Servers
                .CountAsync(cancellationToken);
            return count;
        }
    }
}