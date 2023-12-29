using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Queries
{
    public record GetPlayerCountQuery : ICachedQuery<int>
    {
        public string CacheKey => "player_count";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetPlayerCountQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetPlayerCountQuery, int>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<int> Handle(GetPlayerCountQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Players.CountAsync(cancellationToken: cancellationToken);
        }
    }
}