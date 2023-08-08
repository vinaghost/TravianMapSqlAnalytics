using System.ComponentModel.DataAnnotations.Schema;

namespace MapSqlQuery.Models.Database
{
    public class Player
    {
        // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerId { get; set; }

        // foreign key
        public List<Village> Villages { get; set; } = new();

        public List<PlayerAlliance> Alliances { get; set; } = new();

        // properties
        public int AllianceId { get; set; }

        public string Name { get; set; } = "";
    }
}