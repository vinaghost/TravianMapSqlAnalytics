using MapSqlQuery.Models.Form;
using MapSqlQuery.Models.View;

namespace MapSqlQuery.ViewModels
{
    public class InactivePlayerViewModel
    {
        public List<PlayerPopulation> Players { get; set; } = null!;
        public InactiveFormInput FormInput { get; set; } = null!;
    }
}