using MapSqlQuery.Models.Input;
using MapSqlQuery.Models.Output;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace MapSqlQuery.Models.View
{
    public class VillageFilterViewModel : IServerViewModel
    {
        public string Server { get; set; }
        public int VillageTotal { get; set; }
        public IPagedList<VillageInfo> Villages { get; set; }
        public List<SelectListItem> Alliances { get; set; }
        public List<SelectListItem> Tribes { get; set; }
        public VillageFilterFormInput Input { get; set; }
    }
}