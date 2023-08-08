namespace MapSqlDatabaseUpdate.Service.Interfaces
{
    public interface IGetFileService
    {
        Task<string> GetMapSql(string worldUrl);
    }
}