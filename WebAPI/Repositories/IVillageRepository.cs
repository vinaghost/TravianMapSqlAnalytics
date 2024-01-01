using Core.Models;
using WebAPI.Models.Parameters;

namespace WebAPI.Repositories
{
    public interface IVillageRepository
    {
        IQueryable<Village> GetQueryable(IVillageFilterParameter parameters);
    }
}