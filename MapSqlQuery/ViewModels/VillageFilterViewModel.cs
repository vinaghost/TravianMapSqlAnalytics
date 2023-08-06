using MapSqlQuery.Models.Form;
using MapSqlQuery.Models.View;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MapSqlQuery.ViewModels
{
    public class VillageFilterViewModel
    {
        public List<VillageInfo> Villages = new();
        public List<SelectListItem> Alliances = new();
        public List<SelectListItem> Tribes = new();
        public VillageFormInput FormInput { get; set; } = new();
    }
}