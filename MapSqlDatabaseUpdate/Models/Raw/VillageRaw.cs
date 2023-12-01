namespace MapSqlDatabaseUpdate.Models.Raw
{
    public class VillageRaw
    {
        public VillageRaw(int mapId, int x, int y, int tribe, int villageId, string villageName, int playerId, string playerName, int allianceId, string allianceName, int population, string region, bool isCapital, bool isCity, bool isHarbor, int victoryPoints)
        {
            MapId = mapId;
            X = x;
            Y = y;
            Tribe = tribe;
            VillageId = villageId;
            VillageName = villageName;
            PlayerId = playerId;
            PlayerName = playerName;
            AllianceId = allianceId;
            AllianceName = allianceName;
            Population = population;
            Region = region;
            IsCapital = isCapital;
            IsCity = isCity;
            IsHarbor = isHarbor;
            VictoryPoints = victoryPoints;
        }

        public int MapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Tribe { get; set; }
        public int VillageId { get; set; }
        public string VillageName { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int AllianceId { get; set; }
        public string AllianceName { get; set; }
        public int Population { get; set; }
        public string Region { get; set; }
        public bool IsCapital { get; set; }
        public bool IsCity { get; set; }
        public bool IsHarbor { get; set; }
        public int VictoryPoints { get; set; }
    }
}