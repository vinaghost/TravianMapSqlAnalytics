using System.ComponentModel.DataAnnotations;

namespace MapSqlAspNetCoreMVC.Models.Input
{
    public class PlayerWithAllianceInput : IPagingInput
    {
        [Display(Name = "Days")]
        public int Days { get; set; } = 3;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}