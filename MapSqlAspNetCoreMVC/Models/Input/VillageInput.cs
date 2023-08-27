using System.ComponentModel.DataAnnotations;

namespace MapSqlAspNetCoreMVC.Models.Input
{
    public class VillageInput : IPagingInput
    {
        [Display(Name = "MinPop")]
        public int MinPop { get; set; } = 700;

        [Display(Name = "MaxPop")]
        public int MaxPop { get; set; } = -1;

        [Display(Name = "AllianceId")]
        public int AllianceId { get; set; } = -1;

        [Display(Name = "TribeId")]
        public int TribeId { get; set; } = 0;

        [Display(Name = "X")]
        public int X { get; set; } = 0;

        [Display(Name = "Y")]
        public int Y { get; set; } = 0;

        [Display(Name = "IgnoreCapital")]
        public bool IgnoreCapital { get; set; } = false;

        [Display(Name = "IgnoreNormalVillage")]
        public bool IgnoreNormalVillage { get; set; } = false;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}