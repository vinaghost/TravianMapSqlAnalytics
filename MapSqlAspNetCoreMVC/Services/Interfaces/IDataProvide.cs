using MapSqlQuery.Models.Input;
using MapSqlQuery.Models.Output;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MapSqlQuery.Services.Interfaces
{
    public interface IDataProvide
    {
        List<SelectListItem> GetAllianceSelectList();

        List<DateTime> GetDateBefore(int days);

        List<PlayerPopulation> GetInactivePlayerData(InactiveFormInput input);

        PlayerInfo GetPlayerInfo(PlayerLookupInput input);

        List<SelectListItem> GetTribeSelectList();

        List<VillageInfo> GetVillageData(VillageFilterFormInput input);
    }
}