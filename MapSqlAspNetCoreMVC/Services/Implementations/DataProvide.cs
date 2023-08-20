using MainCore;
using MapSqlAspNetCoreMVC.Models;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MapSqlAspNetCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.Services.Implementations
{
    public class DataProvide : IDataProvide
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

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

        public DataProvide(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public List<PlayerWithPopulation> GetInactivePlayerData(InactiveFormInput input)
        {
            var players = GetPlayersPopulation(input, 0);
            var filterdPlayers = players.Where(x => x.Population.Count > input.Days).ToList();
            return filterdPlayers;
        }

        public List<Village> GetVillageData(VillageFilterFormInput input)
        {
            var villages = GetVillageInfo(input);
            return villages;
        }

        public PlayerWithVillagePopulation GetPlayerInfo(PlayerLookupInput input)
        {
            var player = GetAccountInfo(input);
            return player;
        }

        private List<PlayerWithPopulation> GetPlayersPopulation(InactiveFormInput input, int populationChange)
        {
            using var scoped = _serviceScopeFactory.CreateScope();
            using var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();
            var dates = GetDateBefore(input.Days);
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
                .Where(x => x.Max(x => x.Population) - x.Min(x => x.Population) == populationChange)
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
                Tribe = _tribeNames[x.Tribe],
                Population = x.Population,
                VillageCount = x.VillageCount,
            }).ToList();
            return population;
        }

        private List<Village> GetVillageInfo(VillageFilterFormInput input, CancellationToken cancellationToken = default)
        {
            using var scoped = _serviceScopeFactory.CreateScope();
            using var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();
            var query = context.Villages
                .Where(x => x.Population >= input.MinPop);
            if (input.MaxPop != -1 && input.MaxPop > input.MinPop)
            {
                query = query.Where(x => x.Population <= input.MaxPop);
            }
            if (input.TribeId != 0)
            {
                query = query.Where(x => x.Tribe == input.TribeId);
            }

            var joinQuery = query.Join(context.Players, x => x.PlayerId, x => x.PlayerId, (village, player) => new
            {
                village.VillageId,
                player.AllianceId,
                VillageName = village.Name,
                PlayerName = player.Name,
                TribeId = village.Tribe,
                village.X,
                village.Y,
                village.Population,
                village.IsCapital,
            })
            .Join(context.Alliances, x => x.AllianceId, x => x.AllianceId, (village, alliance) => new
            {
                village.VillageId,
                village.AllianceId,
                AllianceName = alliance.Name,
                village.PlayerName,
                village.VillageName,
                village.TribeId,
                village.X,
                village.Y,
                village.Population,
                village.IsCapital,
            });

            if (input.AllianceId != -1)
            {
                joinQuery = joinQuery.Where(x => x.AllianceId == input.AllianceId);
            }

            var result = joinQuery.AsEnumerable();

            var villagesInfo = new List<Village>();
            var centerCoordinate = new Coordinates(input.X, input.Y);

            foreach (var village in result)
            {
                var villageCoordinate = new Coordinates(village.X, village.Y);
                var distance = centerCoordinate.Distance(villageCoordinate);

                var villageInfo = new Village
                {
                    VillageId = village.VillageId,
                    VillageName = village.VillageName,
                    PlayerName = village.PlayerName,
                    AllianceName = village.AllianceName,
                    Tribe = _tribeNames[village.TribeId],
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

        public List<DateTime> GetDateBefore(int days)
        {
            var dates = new List<DateTime>();
            var today = GetNewestDay();
            for (int i = 0; i <= days; i++)
            {
                var beforeDate = today.AddDays(-i);
                dates.Add(beforeDate);
            }
            return dates;
        }

        private DateTime GetNewestDay()
        {
            using var scoped = _serviceScopeFactory.CreateScope();
            using var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();

            var query = context.VillagesPopulations
                .OrderByDescending(x => x.Date)
                .Select(x => x.Date)
                .FirstOrDefault();
            return query;
        }

        private PlayerWithVillagePopulation GetAccountInfo(PlayerLookupInput input)
        {
            using var scoped = _serviceScopeFactory.CreateScope();
            using var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();

            var playerQuery = context.Players
                .Where(x => x.Name.Equals(input.PlayerName))
                .Include(x => x.Villages)
                .Select(x => new
                {
                    x.PlayerId,
                    x.AllianceId,
                    PlayerName = x.Name,
                    TribeId = x.Villages.First().Tribe,
                })
                .Join(context.Alliances, x => x.AllianceId, x => x.AllianceId, (player, alliance) => new
                {
                    player.PlayerId,
                    player.PlayerName,
                    AllianceName = alliance.Name,
                    player.TribeId,
                })
                .FirstOrDefault();

            if (playerQuery is null)
            {
                return null;
            }

            var playerInfo = new PlayerWithVillagePopulation()
            {
                PlayerName = playerQuery.PlayerName,
                AllianceName = playerQuery.AllianceName,
                Tribe = _tribeNames[playerQuery.TribeId],
                Population = new(),
            };

            var dates = GetDateBefore(input.Days);
            var (minDate, maxDate) = (dates[^1], dates[0]);

            var populationQuery = context.Villages
                 .Where(x => x.PlayerId == playerQuery.PlayerId)
                 .Join(context.VillagesPopulations, x => x.VillageId, x => x.VillageId, (village, population) => new
                 {
                     village.VillageId,
                     VillageName = village.Name,
                     village.X,
                     village.Y,
                     population.Date,
                     population.Population,
                 })
                .Where(x => x.Date >= minDate && x.Date <= maxDate)
                 .GroupBy(x => x.VillageId)
                 .AsEnumerable();

            foreach (var village in populationQuery)
            {
                var villageName = village.First().VillageName;
                var populations = new List<int>();
                foreach (var population in village)
                {
                    populations.Insert(0, population.Population);
                }
                var villageInfo = new VillageWithPopulation()
                {
                    VillageName = villageName,
                    X = village.First().X,
                    Y = village.First().Y,
                    Population = populations,
                };

                playerInfo.Population.Add(villageInfo);
            }

            var maxDays = playerInfo.Population.Max(x => x.Population.Count);

            var totalInfo = new VillageWithPopulation()
            {
                VillageName = VillageWithPopulation.Total,
                Population = new List<int>(new int[maxDays]),
            };

            foreach (var population in playerInfo.Population)
            {
                for (var i = 0; i < maxDays; i++)
                {
                    if (i >= population.Population.Count)
                    {
                        break;
                    }

                    totalInfo.Population[i] += population.Population[i];
                }
            }
            playerInfo.Population.Insert(0, totalInfo);

            return playerInfo;
        }

        public List<SelectListItem> GetAllianceSelectList()
        {
            using var scoped = _serviceScopeFactory.CreateScope();
            using var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>(); var alliances = new List<SelectListItem>
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
                })
                .AsEnumerable();

            alliances.AddRange(query);
            return alliances;
        }

        public List<SelectListItem> GetTribeSelectList()
        {
            return _tribeNamesList;
        }
    }
}