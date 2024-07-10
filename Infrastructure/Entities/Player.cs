using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities
{
    [Index(nameof(Name))]
    public class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int AllianceId { get; set; }
        public string Name { get; set; } = "";
        public int Population { get; set; }
        public int VillageCount { get; set; }

        public ICollection<Village> Villages { get; set; } = [];

        public ICollection<PlayerHistory> History { get; set; } = [];
    }
}