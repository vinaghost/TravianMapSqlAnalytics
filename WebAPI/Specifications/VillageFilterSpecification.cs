using Core.Models;

namespace WebAPI.Specifications
{
    public class VillageFilterSpecification : ISpecification<Player, Village>, ISpecification<Alliance, Village>
    {
        public required List<int> Ids { get; init; }

        public IQueryable<Village> Apply(IQueryable<Player> query)
        {
            var queryVillage = query
                    .Where(x => Ids.Contains(x.PlayerId))
                    .SelectMany(x => x.Villages);
            return queryVillage;
        }

        public IQueryable<Village> Apply(IQueryable<Alliance> query)
        {
            var queryVillage = query
                    .Where(x => Ids.Contains(x.AllianceId))
                    .SelectMany(x => x.Players)
                    .SelectMany(x => x.Villages);
            return queryVillage;
        }
    }
}