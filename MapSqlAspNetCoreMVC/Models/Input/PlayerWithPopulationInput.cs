using System.ComponentModel.DataAnnotations;

namespace MapSqlAspNetCoreMVC.Models.Input
{
    public class PlayerWithPopulationInput : IPagingInput
    {
        [Display(Name = "Days")]
        public int Days { get; set; } = 3;

        [Display(Name = "Tribe")]
        public int Tribe { get; set; } = 0;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}