using System.ComponentModel;

namespace MapSqlQuery.Models.Form
{
    public class VillageFormInput
    {
        public int MinPop { get; set; } = 700;
        public int MaxPop { get; set; } = -1;

        [DisplayName("Select alliance: ")]
        public int AllianceId { get; set; } = -1;

        [DisplayName("Select tribe: ")]
        public int TribeId { get; set; } = 0;

        public int X { get; set; }
        public int Y { get; set; }
    }
}