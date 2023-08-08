using MapSqlDatabaseUpdate.Models;

namespace MapSqlDatabaseUpdate.Service.Interfaces
{
    public interface IUpdateDatabaseService
    {
        Task Execute(List<VillageRaw> villages);
    }
}