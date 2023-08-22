using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;

namespace MapSqlAspNetCoreMVC.Models.View
{
    public class PlayerWithDetailViewModel : IServerViewModel
    {
        public PlayerWithDetailInput Input { get; set; }
        public PlayerWithDetail Player { get; set; }
        public List<DateTime> Dates { get; set; }
        public string Server { get; set; }
    }
}