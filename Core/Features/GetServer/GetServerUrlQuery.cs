using Core.Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.GetServer
{
    public record GetServerUrlQuery(int ServerId) : ICachedQuery<string>
    {
        public string CacheKey => $"{nameof(GetServerUrlQuery)}_{ServerId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServerUrlQueryHandler(ServerListDbContext dbContext) : IRequestHandler<GetServerUrlQuery, string>
    {
        private readonly ServerListDbContext _dbContext = dbContext;

        public async Task<string> Handle(GetServerUrlQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Servers
                .Where(x => x.Id == request.ServerId)
                .Select(x => x.Url)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}