using MapSqlQuery.Models.Form;
using MapSqlQuery.Models.View;

namespace MapSqlQuery.ViewModels
{
    public class InactivePlayerViewModel
    {
        public List<PlayerPopulation> Players { get; set; } = new();
        public InactiveFormInput FormInput { get; set; } = new();
    }
}