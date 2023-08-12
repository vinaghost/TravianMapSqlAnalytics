using MapSqlQuery.Models.Input;
using MapSqlQuery.Models.Output;
using X.PagedList;

namespace MapSqlQuery.Models.View
{
    public class InactivePlayerViewModel : IServerViewModel
    {
        public string Server { get; set; }
        public int PlayerTotal { get; set; }
        public List<DateTime> Dates { get; set; }
        public IPagedList<PlayerPopulation> Players { get; set; }

        public InactiveFormInput Input { get; set; }
    }
}