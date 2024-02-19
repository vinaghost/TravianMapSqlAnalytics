using Core.Entities;
using Core.Features.Shared.Parameters;

namespace Core.Features.Shared.Handler
{
    public class VillageDataQueryHandler(ServerDbContext dbContext)
    {
        protected readonly ServerDbContext _dbContext = dbContext;

        protected IQueryable<Village> GetVillages(IVillagePopulationFilterParameters parameters)
        {
            var query = _dbContext.Villages
               .AsQueryable();
            if (parameters.MaxVillagePopulation != 0)
            {
                query = query
                    .Where(x => x.Population >= parameters.MinVillagePopulation)
                    .Where(x => x.Population <= parameters.MaxVillagePopulation);
            }
            return query;
        }
    }
}