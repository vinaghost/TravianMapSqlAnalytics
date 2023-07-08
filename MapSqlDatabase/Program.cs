using MainCore.Models;
using System.Text.Json;

namespace MapSqlDatabase
{
    internal class Program
    {
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

            if (!Directory.Exists(worldUrl))
            {
                Console.WriteLine($"{worldUrl} doesn't have data");
                return;
            }

            var date = DateTime.Now.ToString("yyyy-MM-dd");
            var fileName = Path.Combine(worldUrl, $"{date}.json");

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"{fileName} is missing");
                return;
            }

            var rawVillages = JsonSerializer.Deserialize<List<VillageRaw>>(File.ReadAllText(fileName));

            var directory = Path.Combine(worldUrl, date);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    var villages = rawVillages.Select(x => new Village(x)).OrderBy(x => x.PlayerId).ToList();
                    Console.WriteLine($"Found {villages.Count} villages in {worldUrl}");
                    var villagesFile = Path.Combine(directory, "villages.json");
                    File.WriteAllText(villagesFile, JsonSerializer.Serialize(villages));
                }),

                Task.Run(() =>
                {
                    var players = rawVillages.DistinctBy(x => x.PlayerId).Select(x => new Player(x)).OrderBy(x => x.Id).ToList();
                    Console.WriteLine($"Found {players.Count} players in {worldUrl}");
                    var playersFile = Path.Combine(directory, "players.json");
                    File.WriteAllText(playersFile, JsonSerializer.Serialize(players));
                }),
                Task.Run(() =>
                {
                    var alliances = rawVillages.DistinctBy(x => x.AllyId).Select(x => new Alliance(x)).OrderBy(x => x.Id).ToList();
                    Console.WriteLine($"Found {alliances.Count} alliances in {worldUrl}");
                    var alliancesFile = Path.Combine(directory, "alliances.json");
                    File.WriteAllText(alliancesFile, JsonSerializer.Serialize(alliances));
                })
            };

            await Task.WhenAll(tasks);
        }
    }
}