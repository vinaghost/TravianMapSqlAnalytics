using MapSqlQuery.Models.Form;
using MapSqlQuery.Models.View;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MapSqlQuery.Services.Interfaces
{
    public interface IDataProvide
    {
        DateTime NewestDate { get; set; }
        string NewestDateStr { get; }

        List<SelectListItem> GetAllianceSelectList();

        Task<List<PlayerPopulation>> GetInactivePlayerData(InactiveFormInput input);

        List<SelectListItem> GetTribeSelectList();

        Task<List<VillageInfo>> GetVillageData(VillageFormInput input);
    }
}