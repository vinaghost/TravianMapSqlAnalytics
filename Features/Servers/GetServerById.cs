using Features.Shared.Dtos;
using Features.Shared.Query;
using Microsoft.EntityFrameworkCore;

namespace Features.Servers
{
    public record GetServerByIdQuery(int ServerId) : ICachedQuery<ServerDto?>
    {
        public string CacheKey => $"{nameof(GetServerByIdQuery)}_{ServerId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServerUrlQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetServerByIdQuery, ServerDto?>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<ServerDto?> Handle(GetServerByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Servers
                .Where(x => x.Id == request.ServerId)
                .Select(x => new ServerDto(x.Id, x.Url))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}