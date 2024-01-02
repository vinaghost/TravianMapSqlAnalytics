using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Player
    {
        // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerId { get; set; }

        // foreign key
        public ICollection<Village> Villages { get; set; }

        public ICollection<PlayerAlliance> Alliances { get; set; }

        // properties
        public int AllianceId { get; set; }

        public string Name { get; set; } = "";
    }
}