namespace Application.Models.Options
{
    public sealed class AppSettings
    {
        public string[] Servers { get; set; } = [];
        public string UrlMapSql { get; set; } = "https://{0}/map.sql";
    }
}