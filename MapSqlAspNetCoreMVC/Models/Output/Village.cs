using System.ComponentModel.DataAnnotations;

namespace MapSqlAspNetCoreMVC.Models.Output
{
    public class Village
    {
        public int VillageId { get; set; }

        [Display(Name = "AllianceName")]
        public string AllianceName { get; set; }

        [Display(Name = "PlayerName")]
        public string PlayerName { get; set; }

        [Display(Name = "VillageName")]
        public string VillageName { get; set; }

        [Display(Name = "Tribe")]
        public string Tribe { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        [Display(Name = "Coordinate")]
        public string Coordinate => $"{X} | {Y}";

        [Display(Name = "Population")]
        public int Population { get; set; }

        [Display(Name = "IsCapital")]
        public bool IsCapital { get; set; }

        [Display(Name = "Distance")]
        public double Distance { get; set; }
    }
}