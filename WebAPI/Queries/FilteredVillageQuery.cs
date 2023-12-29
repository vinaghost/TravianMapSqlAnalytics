using Core;
using MediatR;
using VillageEnitty = Core.Models.Village;

namespace WebAPI.Queries
{
    public record FilteredVillageQuery(List<int> Alliances, List<int> Players) : IQuery<IQueryable<VillageEnitty>>;

    public class VillageQueryHandler(ServerDbContext dbContext) : IRequestHandler<FilteredVillageQuery, IQueryable<VillageEnitty>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IQueryable<VillageEnitty>> Handle(FilteredVillageQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var filterQuery = GetQuery(request);
            return filterQuery;
        }

        private IQueryable<VillageEnitty> GetQuery(FilteredVillageQuery request)
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