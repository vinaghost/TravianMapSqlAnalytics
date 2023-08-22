using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MapSqlAspNetCoreMVC.Services.Interfaces
{
    public interface IDataProvide
    {
        List<SelectListItem> GetAllianceSelectList();

        List<DateTime> GetDateBefore(int days);

        Task<List<PlayerWithPopulation>> GetInactivePlayerData(PlayerWithPopulationInput input);

        DateTime GetNewestDay();

        Task<PlayerWithDetail> GetPlayerInfo(PlayerWithDetailInput input);

        List<SelectListItem> GetTribeSelectList();

        Task<List<Village>> GetVillageData(VillageInput input);
    }
}