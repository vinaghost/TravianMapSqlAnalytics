using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using X.PagedList;

namespace MapSqlAspNetCoreMVC.Models.View
{
    public class HomeViewModel
    {
        public string ServerUrl { get; set; }
        public string Today { get; set; }
        public IPagedList<Server> Servers { get; set; }

        public HomeInput Input { get; set; }
    }
}