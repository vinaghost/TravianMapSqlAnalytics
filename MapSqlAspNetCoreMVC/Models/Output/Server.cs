namespace MapSqlAspNetCoreMVC.Models.Output
{
    public class Server
    {
        public string Url { get; set; }
        public string Region { get; set; }
        public DateTime StartDate { get; set; }
        public int AllianceCount { get; set; }
        public int PlayerCount { get; set; }
        public int VillageCount { get; set; }
    }
}