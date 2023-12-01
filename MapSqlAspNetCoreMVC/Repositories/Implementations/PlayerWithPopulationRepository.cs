using Core;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MapSqlAspNetCoreMVC.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.Repositories.Implementations
{
    public class PlayerWithPopulationRepository : IPlayerWithPopulationRepository
    {
        private readonly IDbContextFactory<ServerDbContext> _contextFactory;

        public PlayerWithPopulationRepository(IDbContextFactory<ServerDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<PlayerWithPopulation>> Get(PlayerWithPopulationInput input)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var dates = context.GetDateBefore(input.Days);
            var (minDate, maxDate) = (dates[^1], dates[0]);

            var query = context.VillagesPopulations
                .Where(x => x.Date >= minDate && x.Date <= maxDate)
                .Join(context.Villages, x => x.VillageId, x => x.VillageId, (population, village) => new
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
                .Join(context.Players, x => x.PlayerId, x => x.PlayerId, (population, player) => new
                {
                    population.PlayerId,
                    PlayerName = player.Name,
                    player.AllianceId,
                    population.Tribe,
                    population.Population,
                    population.VillageCount,
                })
                .Join(context.Alliances, x => x.AllianceId, x => x.AllianceId, (population, alliance) => new
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

            var population = result.Select(x => new PlayerWithPopulation()
            {
                PlayerId = x.PlayerId,
                AllianceName = x.AllianceName,
                PlayerName = x.PlayerName,
                Tribe = Constants.TribeNames[x.Tribe],
                Population = x.Population,
                VillageCount = x.VillageCount,
            }).ToList();
            return population;
        }
    }
}