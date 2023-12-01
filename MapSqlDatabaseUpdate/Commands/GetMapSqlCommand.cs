using MapSqlDatabaseUpdate.Extensions;
using MapSqlDatabaseUpdate.Models.Raw;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MapSqlDatabaseUpdate.Commands
{
    public class GetMapSqlCommand : IRequest<List<VillageRaw>>
    {
        public string Url { get; }

        public GetMapSqlCommand(string url)
        {
            Url = url;
        }
    }

    public class GetMapSqlCommandHandler : IRequestHandler<GetMapSqlCommand, List<VillageRaw>>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GetMapSqlCommandHandler> _logger;

        public GetMapSqlCommandHandler(HttpClient httpClient, ILogger<GetMapSqlCommandHandler> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<VillageRaw>> Handle(GetMapSqlCommand request, CancellationToken cancellationToken)
        {
            var url = $"https://{request.Url}";
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode) return [];
            }
            catch
            {
                return [];
            }

            using var responseStream = await _httpClient.GetStreamAsync($"{url}/map.sql", cancellationToken);
            using var reader = new StreamReader(responseStream);
            var villages = new List<VillageRaw>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var village = GetVillage(line);
                if (village is null) continue;
                villages.Add(village);
            }
            return villages;
        }

        private static VillageRaw GetVillage(string line)
        {
            if (string.IsNullOrEmpty(line)) return null;
            var villageLine = line.Remove(0, 30);
            villageLine = villageLine.Remove(villageLine.Length - 2, 2);
            var fields = villageLine.ParseLine();

            var mapId = int.Parse(fields[0]);
            var x = int.Parse(fields[1]);
            var y = int.Parse(fields[2]);
            var tribe = int.Parse(fields[3]);
            var villageId = int.Parse(fields[4]);
            var villageName = fields[5];
            var playerId = int.Parse(fields[6]);
            var playerName = fields[7];
            var allianceId = int.Parse(fields[8]);
            var allianceName = fields[9];
            var population = int.Parse(fields[10]);
            var region = fields[11];
            var isCapital = fields[12].Equals("TRUE");
            var isCity = fields[13].Equals("TRUE");
            var isHarbor = fields[14].Equals("TRUE");
            var victoryPoints = fields[15].Equals("NULL") ? 0 : int.Parse(fields[15]);
            return new VillageRaw(mapId, x, y, tribe, villageId, villageName, playerId, playerName, allianceId, allianceName, population, region, isCapital, isCity, isHarbor, victoryPoints);
        }
    }
}