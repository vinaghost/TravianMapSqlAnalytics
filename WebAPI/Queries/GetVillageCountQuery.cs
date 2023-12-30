using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.Extensions;
using WebAPI.Models.Parameters;

namespace WebAPI.Queries
{
    public record GetVillageCountQuery(List<int> Alliances, List<int> Players, List<int> Villages, int MinPopulation, int MaxPopulation) : ICachedQuery<int>, IVillageFilterParameter
    {
        public string CacheKey
        {
            get
            {
                if (Alliances.Count > 0)
                {
                    return $"{nameof(GetVillageCountQuery)}_{MinPopulation}_{MaxPopulation}_alliance_{string.Join(',', Alliances)}";
                }
                else if (Players.Count > 0)
                {
                    return $"{nameof(GetVillageCountQuery)}_{MinPopulation}_{MaxPopulation}_player_{string.Join(',', Players)}";
                }
                else if (Villages.Count > 0)
                {
                    return $"{nameof(GetVillageCountQuery)}_{MinPopulation}_{MaxPopulation}_village_{string.Join(',', Villages)}";
                }
                else
                {
                    return $"{nameof(GetVillageCountQuery)}_{MinPopulation}_{MaxPopulation}";
                }
            }
        }

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetVillageCountQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetVillageCountQuery, int>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<int> Handle(GetVillageCountQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.GetQueryable(request)
                            .CountAsync(cancellationToken: cancellationToken);
        }
    }
}