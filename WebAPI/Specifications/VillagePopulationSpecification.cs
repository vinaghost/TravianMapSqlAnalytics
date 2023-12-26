using Core.Models;

namespace WebAPI.Specifications
{
    public class VillagePopulationSpecification : ISpecification<Village, Village>
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
    }
}