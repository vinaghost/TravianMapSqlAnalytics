using Core;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MediatR;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetPlayerWithPopulationByInputQuery(PlayerWithPopulationInput Input) : IRequest<List<PlayerWithPopulation>>;

    public class GetPlayerWithPopulationByInputQueryHandler : IRequestHandler<GetPlayerWithPopulationByInputQuery, List<PlayerWithPopulation>>
    {
        private readonly ServerDbContext _context;

        public GetPlayerWithPopulationByInputQueryHandler(ServerDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlayerWithPopulation>> Handle(GetPlayerWithPopulationByInputQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var input = request.Input;
            var dates = _context.GetDateBefore(input.Days);

            var (minDate, maxDate) = (dates[^1], dates[0]);

            var query = _context.VillagesPopulations
                .Where(x => x.Date >= minDate && x.Date <= maxDate)
                .Join(_context.Villages, x => x.VillageId, x => x.VillageId, (population, village) => new
                {
                    village.PlayerId,
                    population.Date,
                    population.Population,
                    village.Tribe,
                })
                .GroupBy(x => new { x.PlayerId, x.Date })
                .Select(x => new
                {
                    x.Key.Date,
                    x.Key.PlayerId,
                    x.First().Tribe,
                    Population = x.Sum(x => x.Population),
                    VillageCount = x.Count(),
                })
                .GroupBy(x => x.PlayerId)
                .Select(x => new
                {
                    PlayerId = x.Key,
                    Tribe = x.Select(x => x.Tribe).First(),
                    Population = x.Select(x => x.Population).ToList(),
                    VillageCount = x.Select(x => x.VillageCount).First(),
                })
                .Join(_context.Players, x => x.PlayerId, x => x.PlayerId, (population, player) => new
                {
                    population.PlayerId,
                    PlayerName = player.Name,
                    player.AllianceId,
                    population.Tribe,
                    population.Population,
                    population.VillageCount,
                })
                .Join(_context.Alliances, x => x.AllianceId, x => x.AllianceId, (population, alliance) => new
                {
                    population.PlayerId,
                    population.PlayerName,
                    AllianceName = alliance.Name,
                    population.Tribe,
                    population.Population,
                    population.VillageCount,
                })
                .OrderByDescending(x => x.VillageCount);

            var result = query.AsEnumerable()
                .OrderByDescending(x => x.Population[0]);

            var population = result
                .Where(x => x.Population.Count > input.Days && x.Population.Max() - x.Population.Min() == 0)
                .Select(x => new PlayerWithPopulation()
                {
                    PlayerId = x.PlayerId,
                    AllianceName = x.AllianceName,
                    PlayerName = x.PlayerName,
                    Tribe = Constants.TribeNames[x.Tribe],
                    Population = x.Population,
                    VillageCount = x.VillageCount,
                })
                .ToList();
            return population;
        }
    }
}