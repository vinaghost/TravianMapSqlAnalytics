using System.ComponentModel.DataAnnotations;

namespace MapSqlAspNetCoreMVC.Models.Output
{
    public class PlayerWithAlliance
    {
        public int PlayerId { get; set; }

        [Display(Name = "PlayerName")]
        public string PlayerName { get; set; }

        [Display(Name = "AllianceChangeNumber")]
        public int AllianceChangeNumber { get; set; }

        public List<string> AllianceNames { get; set; }
    }
}