using MapSqlQuery.Models;
using MapSqlQuery.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MapSqlQuery.Services.Implementations
{
    public class DataProvide : IDataProvide
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public DataProvide(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private DateTime _newestDate;
        private string _newestDateStr = "";

        public DateTime NewestDate
        {
            get => _newestDate;
            set
            {
                _newestDate = value;
                _newestDateStr = $"{value:yyyy - MM-dd}";
            }
        }

        public string NewestDateStr => _newestDateStr;

        public async Task<List<PlayerPopulation>> GetPlayerData(DateTime date, int days = 3, int tribe = 0, int minChange = 0, int maxChange = 1)
        {
            var players = await GetPlayersAsync();
            var playerPopulations = await GetPlayersPopulation(players, date, days);

            foreach (var player in playerPopulations)
            {
                player.PopulationChange = player.Population[^1] - player.Population[0];
            }

            var filterPopulations = playerPopulations.Where(x => x.PopulationChange >= minChange && x.PopulationChange <= maxChange);
            if (tribe != 0)
            {
                filterPopulations = filterPopulations.Where(x => x.TribeId == tribe);
            }

            var orderedPopulations = filterPopulations.OrderByDescending(x => x.VillageCount).ThenBy(x => x.PopulationChange).ThenByDescending(x => x.Population[0]).ToList();
            return orderedPopulations;
        }

        private async Task<List<Player>> GetPlayersAsync(CancellationToken cancellationToken = default)
        {
            using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var players = await context.Players
                .Include(x => x.Villages)
                .Include(x => x.Populations)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);
            return players;
        }

        private async Task<List<PlayerPopulation>> GetPlayersPopulation(List<Player> players, DateTime date, int days, CancellationToken cancellationToken = default)
        {
            using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var playerPopulations = new List<PlayerPopulation>();
            foreach (var player in players)
            {
                var alliance = await context.Alliances.FindAsync(new object?[] { player.AllianceId }, cancellationToken: cancellationToken);
                var playerPopulation = new PlayerPopulation
                {
                    PlayerId = player.PlayerId,
                    PlayerName = player.Name,
                    AllianceName = alliance?.Name ?? "",
                    TribeId = player.Villages[0].Tribe,
                    VillageCount = player.Villages.Count,
                };
                playerPopulation.Population.Add(player.Villages.Sum(x => x.Pop));

                for (int i = 0; i < days; i++)
                {
                    var beforeDate = date.AddDays(-(i + 1));
                    var playerVillages = player.Populations.Where(x => x.Date == beforeDate).ToList();
                    playerPopulation.Population.Add(playerVillages.Sum(x => x.Population));
                }
                playerPopulations.Add(playerPopulation);
            }
            return playerPopulations.ToList();
        }
    }
}