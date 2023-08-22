using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MapSqlAspNetCoreMVC.Services.Interfaces
{
    public interface IDataProvide
    {
        List<SelectListItem> GetAllianceSelectList();

        List<DateTime> GetDateBefore(int days);

        Task<List<PlayerWithPopulation>> GetInactivePlayerData(InactiveFormInput input);

        DateTime GetNewestDay();

        Task<PlayerWithVillagePopulation> GetPlayerInfo(PlayerLookupInput input);

        List<SelectListItem> GetTribeSelectList();

        Task<List<Village>> GetVillageData(VillageFilterFormInput input);
    }
}