using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VillageEnitty = Core.Models.Village;

namespace WebAPI.Queries
{
    public interface IVillageFilterdRequest
    {
        List<int> Alliances { get; }
        List<int> Players { get; }
    }

    public record GetVillageCountQuery(List<int> Alliances, List<int> Players) : ICachedQuery<int>, IVillageFilterdRequest
    {
        public string CacheKey => $"village_count_{string.Join(',', Alliances)}_{string.Join(',', Players)}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
    public record FilteredVillageQuery(List<int> Alliances, List<int> Players) : IQuery<IQueryable<VillageEnitty>>, IVillageFilterdRequest;

    public class VillageQueryHandler(ServerDbContext dbContext) : IRequestHandler<FilteredVillageQuery, IQueryable<VillageEnitty>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IQueryable<VillageEnitty>> Handle(FilteredVillageQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var filterQuery = GetQuery(request);
            return filterQuery;
        }

        public async Task<int> Handle(GetVillageCountQuery request, CancellationToken cancellationToken)
        {
            var filterQuery = GetQuery(request);
            return await filterQuery.CountAsync(cancellationToken: cancellationToken);
        }

        private IQueryable<VillageEnitty> GetQuery(IVillageFilterdRequest request)
        {
            if (request.Alliances.Count > 0)
            {
                return _dbContext.Alliances
                     .Where(x => request.Alliances.Contains(x.AllianceId))
                     .SelectMany(x => x.Players)
                     .SelectMany(x => x.Villages);
            }
            else if (request.Players.Count > 0)
            {
                return _dbContext.Players
                     .Where(x => request.Players.Contains(x.PlayerId))
                     .SelectMany(x => x.Villages);
            }
            else
            {
                return _dbContext.Villages.AsQueryable();
            }
        }
    }
}