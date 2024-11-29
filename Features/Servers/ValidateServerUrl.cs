using Features.Shared.Query;
using Microsoft.EntityFrameworkCore;

namespace Features.Servers
{
    public record ValidateServerUrlQuery(string ServerUrl) : ICachedQuery<bool>
    {
        public string CacheKey => $"{nameof(ValidateServerUrlQuery)}_{ServerUrl}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class ValidateServerQueryHandler(ServerDbContext dbContext) : IRequestHandler<ValidateServerUrlQuery, bool>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<bool> Handle(ValidateServerUrlQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Servers
                .Where(x => x.Url == request.ServerUrl)
                .AnyAsync(cancellationToken);
        }
    }
}