using System.ComponentModel.DataAnnotations.Schema;

namespace MainCore.DatabaseModels
{
    public class Alliance
    {
        // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AllianceId { get; set; }

        // foreign key
        public List<Player> Players { get; set; } = new();

        // properties
        public string Name { get; set; } = "";
    }
}