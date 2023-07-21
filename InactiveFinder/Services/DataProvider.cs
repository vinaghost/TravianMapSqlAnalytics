using InactiveFinder.Models;
using MainCore.Models;
using System.Text.Json;

namespace InactiveFinder.Services
{
    public class DataProvider
    {
        private readonly HttpClient _httpClient;

        private const string _serverUrl = "ts8.x1.arabics.travian.com";

        public string ServerUrl => _serverUrl;
        private const string _api = "https://raw.githubusercontent.com/vinaghost/TravianArabics8Data/main/ts8.x1.arabics.travian.com/";

        public DataProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DateTime> GetNewestDateTime()
        {
            var now = DateTime.Today.ToString("yyyy-MM-dd");

            var response = await _httpClient.GetAsync($"{_serverUrl}/{now}");
            if (response.IsSuccessStatusCode) return DateTime.Today;
            var yesterday = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            response = await _httpClient.GetAsync($"{_serverUrl}/{yesterday}");
            if (response.IsSuccessStatusCode) return DateTime.Today.AddDays(-1);
            return DateTime.MinValue;
        }

        public async Task<List<PlayerPopulation>> GetPlayerData(DateTime dateTime, int days = 3, int tribe = 0, int minChange = 0, int maxChange = 1)
        {
            var alliances = await GetAlliances(dateTime);
            var players = await GetPlayers(dateTime);
            var villages = await GetVillages(dateTime);

            var playerPopulations = new List<PlayerPopulation>();
            foreach (var player in players)
            {
                var alliance = alliances.FirstOrDefault(x => x.Id == player.AllianceId);
                var playerVillages = villages.Where(x => x.PlayerId == player.Id).ToList();
                var population = playerVillages.Sum(x => x.Pop);
                var playerPopulation = new PlayerPopulation
                {
                    PlayerId = player.Id,
                    PlayerName = player.Name,
                    AllianceName = alliance?.Name,
                    TribeId = playerVillages[0].Tribe,
                    VillageCount = playerVillages.Count,
                };
                playerPopulation.Population.Add(population);
                playerPopulations.Add(playerPopulation);
            }

            for (int i = 0; i < days; i++)
            {
                var beforeDate = dateTime.AddDays(-1 * (i + 1));
                var villagesBeforeDate = await GetVillages(beforeDate);
                foreach (var player in playerPopulations)
                {
                    var playerVillages = villagesBeforeDate.Where(x => x.PlayerId == player.PlayerId).ToList();
                    var population = playerVillages.Sum(x => x.Pop);
                    player.Population.Add(population);
                }
            }

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

        private async Task<List<Alliance>> GetAlliances(DateTime dateTime)
        {
            var dateTimeStr = dateTime.ToString("yyyy-MM-dd");
            var dir = Path.Combine(AppContext.BaseDirectory, dateTimeStr);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var filePath = Path.Combine(dateTimeStr, "alliances.json");
            if (!File.Exists(filePath))
            {
                var response = await _httpClient.GetAsync($"{_api}/{dateTimeStr}/alliances.json");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    await File.WriteAllTextAsync($"{dir}/alliances.json", content);

                    var result = await response.Content.ReadFromJsonAsync<List<Alliance>>();
                    return result ?? new List<Alliance>();
                }
            }
            else
            {
                var content = await File.ReadAllTextAsync(filePath);
                var result = JsonSerializer.Deserialize<List<Alliance>>(content);
                return result ?? new List<Alliance>();
            }
            return new List<Alliance>();
        }

        private async Task<List<Player>> GetPlayers(DateTime dateTime)
        {
            var dateTimeStr = dateTime.ToString("yyyy-MM-dd");
            var dir = Path.Combine(AppContext.BaseDirectory, dateTimeStr);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var filePath = Path.Combine(dateTimeStr, "players.json");
            if (!File.Exists(filePath))
            {
                var response = await _httpClient.GetAsync($"{_api}/{dateTimeStr}/players.json");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    await File.WriteAllTextAsync($"{dir}/players.json", content);

                    var result = await response.Content.ReadFromJsonAsync<List<Player>>();
                    return result ?? new List<Player>();
                }
            }
            else
            {
                var content = await File.ReadAllTextAsync(filePath);
                var result = JsonSerializer.Deserialize<List<Player>>(content);
                return result ?? new List<Player>();
            }
            return new List<Player>();
        }

        private async Task<List<Village>> GetVillages(DateTime dateTime)
        {
            var dateTimeStr = dateTime.ToString("yyyy-MM-dd");
            var dir = Path.Combine(AppContext.BaseDirectory, dateTimeStr);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var filePath = Path.Combine(dateTimeStr, "villages.json");
            if (!File.Exists(filePath))
            {
                var response = await _httpClient.GetAsync($"{_api}/{dateTimeStr}/villages.json");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    await File.WriteAllTextAsync($"{dir}/villages.json", content);

                    var result = await response.Content.ReadFromJsonAsync<List<Village>>();
                    return result ?? new List<Village>();
                }
            }
            else
            {
                var content = await File.ReadAllTextAsync(filePath);
                var result = JsonSerializer.Deserialize<List<Village>>(content);
                return result ?? new List<Village>();
            }
            return new List<Village>();
        }
    }
}