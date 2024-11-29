using Features.Shared.Query;
using Microsoft.EntityFrameworkCore;

namespace Features.Servers
{
    public record GetMostPlayerServerQuery : ICachedQuery<string>
    {
        public string CacheKey => nameof(GetMostPlayerServerQuery);

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetMostPlayerServerQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetMostPlayerServerQuery, string>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<string> Handle(GetMostPlayerServerQuery request, CancellationToken cancellationToken)
        {
            var server = await _dbContext.Servers
                .OrderByDescending(x => x.PlayerCount)
                .Select(x => x.Url)
                .FirstOrDefaultAsync(cancellationToken);
            return server ?? "";
        }
    }
}