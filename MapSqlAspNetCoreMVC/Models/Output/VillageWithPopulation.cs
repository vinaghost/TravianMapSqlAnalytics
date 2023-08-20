using System.ComponentModel.DataAnnotations;

namespace MapSqlAspNetCoreMVC.Models.Output
{
    public class VillageWithPopulation
    {
        public const string Total = "Total";

        [Display(Name = "VillageName")]
        public string VillageName { get; set; }

        public List<int> Population { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        [Display(Name = "Coordinate")]
        public string Coordinate => $"{X}|{Y}";
    }
}