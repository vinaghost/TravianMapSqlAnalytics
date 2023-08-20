using System.ComponentModel.DataAnnotations;

namespace MapSqlAspNetCoreMVC.Models.Output
{
    public class PlayerWithPopulation
    {
        [Display(Name = "PlayerName")]
        public string PlayerName { get; set; }

        [Display(Name = "Tribe")]
        public string Tribe { get; set; }

        [Display(Name = "AllianceName")]
        public string AllianceName { get; set; }

        [Display(Name = "Population")]
        public List<VillageWithPopulation> Population { get; set; }
    }
}