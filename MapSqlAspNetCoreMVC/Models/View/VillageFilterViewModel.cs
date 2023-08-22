using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace MapSqlAspNetCoreMVC.Models.View
{
    public class VillageFilterViewModel : IServerViewModel
    {
        public string Server { get; set; }
        public int VillageTotal { get; set; }
        public IPagedList<Village> Villages { get; set; }
        public List<SelectListItem> Alliances { get; set; }
        public List<SelectListItem> Tribes { get; set; }
        public VillageInput Input { get; set; }
    }
}