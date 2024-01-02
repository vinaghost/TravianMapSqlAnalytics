using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class AllianceEntity
    {
        // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AllianceId { get; set; }

        // foreign key
        public ICollection<PlayerEntity> Players { get; set; }

        // properties
        public string Name { get; set; } = "";
    }
}