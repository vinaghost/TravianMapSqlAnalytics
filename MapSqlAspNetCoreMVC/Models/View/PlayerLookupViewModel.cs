using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;

namespace MapSqlAspNetCoreMVC.Models.View
{
    public class PlayerLookupViewModel : IServerViewModel
    {
        public PlayerLookupInput Input { get; set; }
        public PlayerWithPopulation Player { get; set; }
        public List<DateTime> Dates { get; set; }
        public string Server { get; set; }
    }
}