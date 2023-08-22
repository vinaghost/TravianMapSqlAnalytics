using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using X.PagedList;

namespace MapSqlAspNetCoreMVC.Models.View
{
    public class InactivePlayerViewModel : IServerViewModel
    {
        public string Server { get; set; }
        public int PlayerTotal { get; set; }
        public List<DateTime> Dates { get; set; }
        public IPagedList<PlayerWithPopulation> Players { get; set; }

        public PlayerWithPopulationInput Input { get; set; }
    }
}