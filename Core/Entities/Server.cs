using Microsoft.EntityFrameworkCore;

namespace Core.Entities
{
    [Index(nameof(Url))]
    public class Server
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime LastUpdate { get; set; }
        public int AllianceCount { get; set; }
        public int PlayerCount { get; set; }
        public int VillageCount { get; set; }
        public int OasisCount { get; set; }
    }
}