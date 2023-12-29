using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Queries
{
    public record ValidateServerQuery(string Server) : ICachedQuery<bool>
    {
        public string CacheKey => $"Validate_{Server}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class ValidateServerQueryHandler(ServerListDbContext dbContext) : IRequestHandler<ValidateServerQuery, bool>
    {
        private readonly ServerListDbContext _dbContext = dbContext;

        public async Task<bool> Handle(ValidateServerQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Servers
                            .Where(x => x.Url == request.Server)
                            .AnyAsync(cancellationToken: cancellationToken);
        }
    }
}