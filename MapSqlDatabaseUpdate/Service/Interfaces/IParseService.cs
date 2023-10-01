using MapSqlDatabaseUpdate.Models;

namespace MapSqlDatabaseUpdate.Service.Interfaces
{
    public interface IParseService
    {
        List<VillageRaw> GetVillages(string lines);
    }
}