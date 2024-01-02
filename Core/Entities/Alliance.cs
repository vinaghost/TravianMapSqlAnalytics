using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Alliance
    {
        // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AllianceId { get; set; }

        // foreign key
        public ICollection<Player> Players { get; set; }

        // properties
        public string Name { get; set; } = "";
    }
}