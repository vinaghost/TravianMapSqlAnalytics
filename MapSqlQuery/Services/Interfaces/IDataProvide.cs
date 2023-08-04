using MapSqlQuery.Models;

namespace MapSqlQuery.Services.Interfaces
{
    public interface IDataProvide
    {
        DateTime NewestDate { get; set; }
        string NewestDateStr { get; }

        Task<List<PlayerPopulation>> GetPlayerData(DateTime dateTime, int days = 3, int tribe = 0, int minChange = 0, int maxChange = 1);
    }
}