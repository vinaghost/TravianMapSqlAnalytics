using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;

namespace MapSqlAspNetCoreMVC.Repositories.Interfaces
{
    public interface IVillageRepository : IRepository<VillageInput, List<Village>>
    {
    }
}