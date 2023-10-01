using MapSqlDatabaseUpdate.Extensions;
using MapSqlDatabaseUpdate.Models;
using MapSqlDatabaseUpdate.Service.Interfaces;

namespace MapSqlDatabaseUpdate.Service.Implementations
{
    public class ParseService : IParseService
    {
        public List<VillageRaw> GetVillages(string lines)
        {
            var villageLines = lines.Split("\n");
            var villages = new List<VillageRaw>();
            foreach (var line in villageLines)
            {
                var village = GetVillage(line);
                if (village is null) continue;
                villages.Add(village);
            }
            return villages;
        }

        private VillageRaw GetVillage(string line)
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
            var victoryPoints = fields[15].Equals("NULL") ? 0 : int.Parse(fields[14]);
            return new VillageRaw(mapId, x, y, tribe, villageId, villageName, playerId, playerName, allianceId, allianceName, population, region, isCapital, isCity, isHarbor, victoryPoints);
        }
    }
}