using System.ComponentModel;

namespace MapSqlQuery.Models.Output
{
    public class PlayerInfo
    {
        [DisplayName("Player name")]
        public string PlayerName { get; set; }

        [DisplayName("Tribe")]
        public string Tribe { get; set; }

        [DisplayName("Alliance name")]
        public string AllianceName { get; set; }

        [DisplayName("Population")]
        public List<VillageInfo> Population { get; set; }

        public class VillageInfo
        {
            public const string Total = "Total";
            public string VillageName { get; set; }

            public List<int> Population { get; set; }

            public int X { get; set; }
            public int Y { get; set; }
            public string Coordinate => $"{X}|{Y}";
        }
    }
}