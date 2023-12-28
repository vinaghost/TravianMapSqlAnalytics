using Core.Models;

namespace WebAPI.Specifications.Players
{
    public class PlayerFilterSpecification : ISpecification<Alliance, Player>
    {
        public required List<int> Ids { get; init; }

        public IQueryable<Player> Apply(IQueryable<Alliance> query)
        {
            var queryPlayer = query
                    .Where(x => Ids.Contains(x.AllianceId))
                    .SelectMany(x => x.Players);
            return queryPlayer;
        }
    }
}