using MapSqlQuery.Models;
using MapSqlQuery.Models.Form;

namespace MapSqlQuery.ViewModels
{
    public class InactivePlayerViewModel
    {
        public List<PlayerPopulation> Players { get; set; } = null!;
        public InactiveFormInput FormInput { get; set; } = null!;
    }
}