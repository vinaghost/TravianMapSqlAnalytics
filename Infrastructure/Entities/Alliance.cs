using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities
{
    [Index(nameof(Name))]
    public class Alliance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public ICollection<Player> Players { get; set; } = [];
        public ICollection<AllianceHistory> History { get; set; } = [];
        public string Name { get; set; } = "";
        public int PlayerCount { get; set; }
    }
}