using System.ComponentModel;

namespace MapSqlAspNetCoreMVC.Models.Input
{
    public class VillageFilterFormInput : IPagingInput
    {
        public int MinPop { get; set; } = 700;
        public int MaxPop { get; set; } = -1;

        [DisplayName("Select alliance: ")]
        public int AllianceId { get; set; } = -1;

        [DisplayName("Select tribe: ")]
        public int TribeId { get; set; } = 0;

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}