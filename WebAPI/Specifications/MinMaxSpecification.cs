using WebAPI.Models.Output;
using VillageEntity = Core.Models.Village;

namespace WebAPI.Specifications
{
    public class MinMaxSpecification : ISpecification<VillageEntity>, ISpecification<Player>
    {
        public required int Min { get; init; }
        public required int Max { get; init; }

        public IQueryable<VillageEntity> Apply(IQueryable<VillageEntity> query)
        {
            query
               .Where(x => x.Population >= Min)
               .Where(x => x.Population <= Max);
            return query;
        }

        public IQueryable<Player> Apply(IQueryable<Player> query)
        {
            query
               .Where(x => x.Population >= Min)
               .Where(x => x.Population <= Max);
            return query;
        }
    }
}