using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MapSqlAspNetCoreMVC.Services.Interfaces
{
    public interface IDataProvide
    {
        List<SelectListItem> GetAllianceSelectList();

        List<DateTime> GetDateBefore(int days);

        List<PlayerWithPopulation> GetInactivePlayerData(InactiveFormInput input);

        PlayerWithVillagePopulation GetPlayerInfo(PlayerLookupInput input);

        List<SelectListItem> GetTribeSelectList();

        List<Village> GetVillageData(VillageFilterFormInput input);
    }
}