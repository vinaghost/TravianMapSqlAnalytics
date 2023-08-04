namespace MapSqlQuery.Services.Interfaces
{
    public interface IDataUpdate
    {
        Task<DateTime> GetNewestDate();

        Task UpdateAlliances();

        Task UpdatePlayer();

        Task UpdatePopulation(DateTime dateTime);

        Task UpdateVillage();
    }
}