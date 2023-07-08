using MainCore.Parsers;
using System.Text.Json;

namespace MapSqlDownloader
{
    internal class Program
    {
        private static HttpClient _httpClient;

        private static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide world url");
                return;
            }
            var worldUrl = args[0];
            if (string.IsNullOrEmpty(worldUrl))
            {
                Console.WriteLine("Please provide world url");
                return;
            }

            Console.WriteLine("Hello, World!");
            Init();

            var villageLines = await GetMapSql(worldUrl);
            if (string.IsNullOrEmpty(villageLines))
            {
                Console.WriteLine($"{worldUrl} doesn't any village in map.sql");
                return;
            }
            var villages = MapSqlParser.GetVillages(villageLines);

            Console.WriteLine($"Found {villages.Count} villages in {worldUrl}");

            if (!Directory.Exists(worldUrl)) Directory.CreateDirectory(worldUrl);

            var date = DateTime.Now.ToString("yyyy-MM-dd");
            var fileName = Path.Combine(worldUrl, $"{date}.json");

            File.WriteAllText(fileName, JsonSerializer.Serialize(villages));

            Dispose();
        }

        private static void Init()
        {
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromSeconds(60),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(20),
                MaxConnectionsPerServer = 10
            };
            _httpClient = new HttpClient(socketsHandler);
        }

        private static void Dispose()
        {
            _httpClient.Dispose();
        }

        private static async Task<string> GetMapSql(string worldUrl)
        {
            try
            {
                var result = await _httpClient.GetStringAsync($"https://{worldUrl}/map.sql");
                return result;
            }
            catch
            {
                return "";
            }
        }
    }
}