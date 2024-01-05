using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Queries
{
    public record GetServerCountQuery : ICachedQuery<int>
    {
        public string CacheKey => nameof(GetServerCountQuery);

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServerCountQueryHandler(ServerListDbContext dbContext) : IRequestHandler<GetServerCountQuery, int>
    {
        private readonly ServerListDbContext _dbContext = dbContext;

        public async Task<int> Handle(GetServerCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _dbContext
                .Servers
                .CountAsync(cancellationToken);
            return count;
        }
    }
}