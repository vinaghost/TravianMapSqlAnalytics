using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.Models.Database
{
    [Index(nameof(Server), IsUnique = true)]
    public class TimeUpdate
    {
        public int Id { get; set; }
        public string Server { get; set; } = "";
        public DateTime LastUpdate { get; set; }
    }
}