using MainCore.Models;

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

        public async Task<List<Player>> GetPlayers(DateTime dateTime)
        {
            var dateTimeStr = dateTime.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetFromJsonAsync<List<Player>>($"{_api}/{dateTimeStr}/players.json");

            return response ?? new List<Player>();
        }
    }
}