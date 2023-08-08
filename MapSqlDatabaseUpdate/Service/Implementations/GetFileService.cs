using MapSqlDatabaseUpdate.Service.Interfaces;

namespace MapSqlDatabaseUpdate.Service.Implementations
{
    public class GetFileService : IGetFileService
    {
        private readonly HttpClient _httpClient;

        public GetFileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// ts1.europe.travian.com -> https://[ts1.europe.travian.com]/map.sql
        /// </summary>
        /// <param name="worldUrl">Example: ts1.europe.travian.com</param>
        /// <returns></returns>
        public async Task<string> GetMapSql(string worldUrl)
        {
            try
            {
                var result = await _httpClient.GetStringAsync($"https://{worldUrl}/map.sql");
                return result;
            }
            catch
            {
                return "";
            }
        }
    }
}