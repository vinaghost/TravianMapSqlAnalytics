using MapSqlDatabaseUpdate.Models;

namespace MapSqlDatabaseUpdate.Service.Interfaces
{
    public interface IParseService
    {
        VillageRaw GetVillage(string line);
        List<VillageRaw> GetVillages(string lines);
    }
}