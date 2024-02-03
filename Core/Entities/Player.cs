using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    [Index(nameof(Name))]
    public class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public ICollection<Village> Villages { get; set; }

        public ICollection<PlayerAllianceHistory> Alliances { get; set; }
        public ICollection<PlayerPopulationHistory> Populations { get; set; }

        public int AllianceId { get; set; }

        public string Name { get; set; } = "";
        public int Population { get; set; }
        public int VillageCount { get; set; }
    }
}