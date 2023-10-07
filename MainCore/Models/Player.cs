using System.ComponentModel.DataAnnotations.Schema;

namespace MainCore.Models
{
    public class Player
    {
        // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerId { get; set; }

        public int AllianceId { get; set; }

        public string Name { get; set; } = "";

        public ICollection<Village> Villages { get; set; }

        public ICollection<PlayerAlliance> Alliances { get; set; }
    }
}