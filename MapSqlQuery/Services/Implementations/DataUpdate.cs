using MapSqlQuery.Models;
using MapSqlQuery.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using AllianceCore = MainCore.Models.Alliance;
using PlayerCore = MainCore.Models.Player;
using VillageCore = MainCore.Models.Village;

namespace MapSqlQuery.Services.Implementations
{
    public class DataUpdate : IDataUpdate
    {
        private readonly HttpClient _httpClient;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly IDataProvide _dataProvide;

        private readonly string _dateUrl;
        private readonly string _contentUrl;

        public DataUpdate(HttpClient httpClient, IDbContextFactory<AppDbContext> contextFactory, IConfiguration configuration, IDataProvide dataProvide)
        {
            _httpClient = httpClient;
            _contextFactory = contextFactory;

            _dataProvide = dataProvide;

            _dateUrl = $"{configuration["DateUrl"]}/{configuration["WorldUrl"]}";
            _contentUrl = $"{configuration["ContentUrl"]}/{configuration["WorldUrl"]}";
        }

        public async Task<DateTime> GetNewestDate()
        {
            var now = DateTime.Today.ToString("yyyy-MM-dd");

            var response = await _httpClient.GetAsync($"{_dateUrl}/{now}");
            if (response.IsSuccessStatusCode) return DateTime.Today;

            var yesterday = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            response = await _httpClient.GetAsync($"{_dateUrl}/{yesterday}");

            if (response.IsSuccessStatusCode) return DateTime.Today.AddDays(-1);
            return DateTime.MinValue;
        }

        public async Task UpdateAlliances()
        {
            var date = _dataProvide.NewestDate;

            var alliances = await GetAlliances(date);

            using var context = _contextFactory.CreateDbContext();
            foreach (var alliance in alliances)
            {
                var allianceDb = await context.Alliances.FindAsync(alliance.AllianceId);
                if (allianceDb is null)
                {
                    context.Add(alliance);
                }
                else
                {
                    allianceDb.Name = alliance.Name;
                }
            }
            context.SaveChanges();
        }

        public async Task UpdatePlayer()
        {
            var date = _dataProvide.NewestDate;

            var players = await GetPlayers(date);

            using var context = _contextFactory.CreateDbContext();
            foreach (var player in players)
            {
                var playerDb = await context.Players.FindAsync(player.PlayerId);
                if (playerDb is null)
                {
                    context.Add(player);
                }
                else
                {
                    playerDb.Name = player.Name;
                    playerDb.AllianceId = player.AllianceId;
                }
            }
            context.SaveChanges();
        }

        public async Task UpdateVillage()
        {
            var date = _dataProvide.NewestDate;

            var villages = await GetVillages(date);

            using var context = _contextFactory.CreateDbContext();
            foreach (var village in villages)
            {
                var villageDb = await context.Villages.FindAsync(village.VillageId);
                if (villageDb is null)
                {
                    context.Add(village);
                }
                else
                {
                    villageDb.PlayerId = village.PlayerId;
                    villageDb.Name = village.Name;
                    villageDb.Tribe = village.Tribe;
                    villageDb.Pop = village.Pop;
                    villageDb.IsCapital = village.IsCapital;
                    villageDb.IsCity = village.IsCity;
                    villageDb.Region = village.Region;
                    villageDb.VictoryPoints = village.VictoryPoints;
                }
            }
            context.SaveChanges();
        }

        public async Task UpdatePopulation(DateTime dateTime)
        {
            var villages = await GetVillagesPopulation(dateTime);

            using var context = _contextFactory.CreateDbContext();
            foreach (var village in villages)
            {
                var playerDb = await context.Players.FindAsync(village.PlayerId);
                if (playerDb is null) continue;
                var villagePopulationDb = await context.VillagesPopulation.FindAsync(village.VillageId, dateTime);
                if (villagePopulationDb is null)
                {
                    context.Add(village);
                }
                else
                {
                    break;
                }
            }
            context.SaveChanges();
        }

        private async Task<List<Alliance>> GetAlliances(DateTime dateTime)
        {
            var dateTimeStr = dateTime.ToString("yyyy-MM-dd");

            var response = await _httpClient.GetAsync($"{_contentUrl}/{dateTimeStr}/alliances.json");
            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadFromJsonAsync<List<AllianceCore>>() ?? new List<AllianceCore>();
                var alliances = result.Select(x => new Alliance
                {
                    AllianceId = x.Id,
                    Name = x.Name,
                }).ToList();
                return alliances;
            }

            return new List<Alliance>();
        }

        private async Task<List<Player>> GetPlayers(DateTime dateTime)
        {
            var dateTimeStr = dateTime.ToString("yyyy-MM-dd");

            var response = await _httpClient.GetAsync($"{_contentUrl}/{dateTimeStr}/players.json");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<PlayerCore>>() ?? new List<PlayerCore>();
                var players = result.Select(x => new Player
                {
                    PlayerId = x.Id,
                    Name = x.Name,
                    AllianceId = x.AllianceId,
                }).ToList();
                return players;
            }
            return new List<Player>();
        }

        private async Task<List<Village>> GetVillages(DateTime dateTime)
        {
            var dateTimeStr = dateTime.ToString("yyyy-MM-dd");

            var response = await _httpClient.GetAsync($"{_contentUrl}/{dateTimeStr}/villages.json");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<VillageCore>>() ?? new List<VillageCore>();
                var villages = result.Select(x => new Village
                {
                    PlayerId = x.PlayerId,
                    VillageId = x.Id,
                    MapId = x.MapId,
                    Name = x.Name,
                    X = x.X,
                    Y = x.Y,
                    Tribe = x.Tribe,
                    Pop = x.Pop,
                    Region = x.Region,
                    IsCapital = x.IsCapital,
                    IsCity = x.IsCity,
                    VictoryPoints = x.VictoryPoints,
                }).ToList();
                return villages;
            }

            return new List<Village>();
        }

        private async Task<List<VillagePopulation>> GetVillagesPopulation(DateTime dateTime)
        {
            var dateTimeStr = dateTime.ToString("yyyy-MM-dd");

            var response = await _httpClient.GetAsync($"{_contentUrl}/{dateTimeStr}/villages.json");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<VillageCore>>() ?? new List<VillageCore>();
                var villages = result.Select(x => new VillagePopulation
                {
                    PlayerId = x.PlayerId,
                    VillageId = x.Id,
                    Population = x.Pop,
                    Date = dateTime,
                }).ToList();
                return villages;
            }

            return new List<VillagePopulation>();
        }
    }
}