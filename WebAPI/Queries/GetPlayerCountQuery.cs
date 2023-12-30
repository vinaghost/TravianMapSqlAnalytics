using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.Extensions;
using WebAPI.Models.Parameters;

namespace WebAPI.Queries
{
    public record GetPlayerCountQuery(List<int> Alliances, List<int> Players) : ICachedQuery<int>, IPlayerFilterParameter
    {
        public string CacheKey
        {
            get
            {
                if (Alliances.Count > 0)
                {
                    return $"{nameof(GetVillageCountQuery)}_alliance_{string.Join(',', Alliances)}";
                }
                else if (Players.Count > 0)
                {
                    return $"{nameof(GetVillageCountQuery)}_player_{string.Join(',', Players)}";
                }
                else
                {
                    return $"{nameof(GetVillageCountQuery)}";
                }
            }
        }
        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class PlayerQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetPlayerCountQuery, int>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<int> Handle(GetPlayerCountQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.GetQueryable(request)
                .CountAsync(cancellationToken: cancellationToken);
        }
    }
}