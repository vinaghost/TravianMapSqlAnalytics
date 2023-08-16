namespace MapSqlQuery.Models.Output
{
    public class VillageInfo
    {
        public int VillageId { get; set; }
        public string AllianceName { get; set; }
        public string PlayerName { get; set; }
        public string VillageName { get; set; }
        public string Tribe { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Coordinate => $"{X} | {Y}";
        public int Population { get; set; }
        public bool IsCapital { get; set; }
        public double Distance { get; set; }
    }
}