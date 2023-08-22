using System.ComponentModel.DataAnnotations;

namespace MapSqlAspNetCoreMVC.Models.Output
{
    public class PlayerWithAlliance
    {
        public int PlayerId { get; set; }

        [Display(Name = "PlayerName")]
        public string PlayerName { get; set; }

        [Display(Name = "Tribe")]
        public string Tribe { get; set; }

        [Display(Name = "VillageCount")]
        public int VillageCount { get; set; }

        public List<string> AllianceName { get; set; }
    }
}