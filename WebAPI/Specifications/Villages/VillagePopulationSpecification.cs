using WebAPI.Models.Output;
using VillageEntity = Core.Models.Village;

namespace WebAPI.Specifications.Villages
{
    public class VillagePopulationSpecification : ISpecification<Village>, ISpecification<VillageEntity>
    {
        public required int Min { get; init; }
        public required int Max { get; init; }

        public IQueryable<Village> Apply(IQueryable<Village> query)
        {
            query
                .Where(x => x.Population >= Min)
                .Where(x => x.Population <= Max);
            return query;
        }

        public IQueryable<VillageEntity> Apply(IQueryable<VillageEntity> query)
        {
            query
               .Where(x => x.Population >= Min)
               .Where(x => x.Population <= Max);
            return query;
        }
    }
}