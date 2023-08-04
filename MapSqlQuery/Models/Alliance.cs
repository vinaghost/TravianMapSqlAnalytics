using System.ComponentModel.DataAnnotations.Schema;

namespace MapSqlQuery.Models
{
    public class Alliance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AllianceId { get; set; }

        public string Name { get; set; } = "";

        public List<Player> Players { get; set; } = new();
    }
}