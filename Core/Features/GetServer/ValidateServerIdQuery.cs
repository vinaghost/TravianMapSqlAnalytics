using Core.Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.GetServer
{
    public record ValidateServerIdQuery(int ServerId) : ICachedQuery<bool>
    {
        public string CacheKey => $"{nameof(ValidateServerIdQuery)}_{ServerId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public record ValidateServerUrlQuery(string ServerUrl) : ICachedQuery<bool>
    {
        public string CacheKey => $"{nameof(ValidateServerUrlQuery)}_{ServerUrl}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class ValidateServerQueryHandler(ServerListDbContext dbContext) : IRequestHandler<ValidateServerIdQuery, bool>, IRequestHandler<ValidateServerUrlQuery, bool>
    {
        private readonly ServerListDbContext _dbContext = dbContext;

        public async Task<bool> Handle(ValidateServerIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Servers
                .Where(x => x.Id == request.ServerId)
                .AnyAsync(cancellationToken);
        }

        public async Task<bool> Handle(ValidateServerUrlQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Servers
                .Where(x => x.Url == request.ServerUrl)
                .AnyAsync(cancellationToken);
        }
    }
}