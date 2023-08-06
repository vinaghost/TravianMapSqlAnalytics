﻿using MapSqlQuery.Models;
using MapSqlQuery.Models.Database;
using MapSqlQuery.Models.Form;
using MapSqlQuery.Models.View;
using MapSqlQuery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MapSqlQuery.Services.Implementations
{
    public class DataProvide : IDataProvide
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly Dictionary<int, string> _tribeNames = new()
        {
            {1, "Romans" },
            {2, "Teutons" },
            {3, "Gauls" },
            {4, "Nature " },
            {5, "Natars" },
            {6, "Egyptians" },
            {7, "Huns" },
            {8, "Spartans" },
        };

        private readonly List<SelectListItem> _tribeNamesList = new()
        {
            new SelectListItem {Value = "0", Text = "All"},
            new SelectListItem {Value = "1", Text = "Romans"},
            new SelectListItem {Value = "2", Text = "Teutons"},
            new SelectListItem {Value = "3", Text = "Gauls"},
            new SelectListItem {Value = "4", Text = "Nature"},
            new SelectListItem {Value = "5", Text = "Natars"},
            new SelectListItem {Value = "6", Text = "Egyptians"},
            new SelectListItem {Value = "7", Text = "Huns"},
            new SelectListItem {Value = "8", Text = "Spartans"},
        };

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

        public async Task<List<PlayerPopulation>> GetInactivePlayerData(DateTime date, int days = 3, int tribe = 0, int minChange = 0, int maxChange = 1)
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

        public async Task<List<VillageInfo>> GetVillageData(VillageFormInput input)
        {
            var villages = await GetVillageInfoAsync(input);
            return villages;
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
                playerPopulation.Population.Add(player.Villages.Sum(x => x.Population));

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

        private async Task<List<VillageInfo>> GetVillageInfoAsync(VillageFormInput input, CancellationToken cancellationToken = default)
        {
            using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var villages = context.Villages.Where(x => x.Population >= input.MinPop);
            if (input.MaxPop != -1 && input.MaxPop > input.MinPop)
            {
                villages = villages.Where(x => x.Population <= input.MaxPop);
            }
            if (input.TribeId != 0)
            {
                villages = villages.Where(x => x.Tribe == input.TribeId);
            }
            var villagesInfo = new List<VillageInfo>();
            var centerCoordinate = new Coordinates(input.X, input.Y);
            foreach (var village in villages)
            {
                var player = await context.Players.FindAsync(new object?[] { village.PlayerId }, cancellationToken: cancellationToken);
                if (player is null) continue;

                var alliance = await context.Alliances.FindAsync(new object?[] { player.AllianceId }, cancellationToken: cancellationToken);
                if (alliance is null) continue;

                if (input.AllianceId != -1 && input.AllianceId != alliance.AllianceId) continue;

                var villageCoordinate = new Coordinates(village.X, village.Y);
                var distance = centerCoordinate.Distance(villageCoordinate);

                var villageInfo = new VillageInfo
                {
                    VillageId = village.VillageId,
                    VillageName = village.Name,
                    PlayerName = player.Name,
                    AllianceName = alliance.Name,
                    Tribe = _tribeNames[village.Tribe],
                    X = village.X,
                    Y = village.Y,
                    Population = village.Population,
                    IsCapital = village.IsCapital,
                    Distance = distance,
                };
                villagesInfo.Add(villageInfo);
            }

            var oredered = villagesInfo.OrderBy(x => x.Distance).ToList();
            return oredered;
        }

        public List<SelectListItem> GetAllianceSelectList()
        {
            using var context = _contextFactory.CreateDbContext();
            var alliances = new List<SelectListItem>
            {
                new SelectListItem { Value = "-1", Text = "All" }
            };

            var query = context.Alliances
                .Include(x => x.Players)
                .OrderByDescending(x => x.Players.Count)
                .Select(x => new SelectListItem
                {
                    Value = $"{x.AllianceId}",
                    Text = x.Name,
                });

            alliances.AddRange(query);
            return alliances;
        }

        public List<SelectListItem> GetTribeSelectList()
        {
            return _tribeNamesList;
        }
    }
}