using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using X.PagedList;

namespace MapSqlAspNetCoreMVC.Models.View
{
    public class PlayerAllianceChangeViewModel : IServerViewModel
    {
        public string Server { get; set; }
        public int PlayerTotal { get; set; }
        public List<DateTime> Dates { get; set; }
        public IPagedList<PlayerWithAlliance> Players { get; set; }

        public PlayerWithAllianceInput Input { get; set; }
    }
}