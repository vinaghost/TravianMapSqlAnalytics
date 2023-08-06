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

        Task<List<PlayerPopulation>> GetInactivePlayerData(DateTime dateTime, int days = 3, int tribe = 0, int minChange = 0, int maxChange = 1);

        List<SelectListItem> GetTribeSelectList();

        Task<List<VillageInfo>> GetVillageData(VillageFormInput input);
    }
}