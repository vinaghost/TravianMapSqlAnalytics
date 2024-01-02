using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class PlayerEntity
    {
        // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerId { get; set; }

        // foreign key
        public ICollection<VillageEntity> Villages { get; set; }

        public ICollection<PlayerAlliance> Alliances { get; set; }

        // properties
        public int AllianceId { get; set; }

        public string Name { get; set; } = "";
    }
}