using System.ComponentModel.DataAnnotations.Schema;

namespace MapSqlQuery.Models
{
    public class Village
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillageId { get; set; }

        public int MapId { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public int Tribe { get; set; }
        public int Pop { get; set; }
        public string Region { get; set; } = "";
        public bool IsCapital { get; set; }
        public bool IsCity { get; set; }
        public int VictoryPoints { get; set; }
    }
}