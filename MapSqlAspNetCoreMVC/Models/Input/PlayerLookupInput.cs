using System.ComponentModel.DataAnnotations;

namespace MapSqlAspNetCoreMVC.Models.Input
{
    public class PlayerLookupInput
    {
        [Display(Name = "PlayerNameTitle")]
        public string PlayerName { get; set; }

        [Display(Name = "DaysTitle")]
        public int Days { get; set; } = 7;
    }
}