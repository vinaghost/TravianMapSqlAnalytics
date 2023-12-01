using System.ComponentModel.DataAnnotations.Schema;

namespace MapSqlDatabaseUpdate.Models.Database
{
    public class Village
    {
        // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillageId { get; set; }

        // foreign key
        public List<VillagePopulation> Populations { get; set; } = new();

        // properties
        public int MapId { get; set; }

        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public int Tribe { get; set; }
        public int Population { get; set; }
        public string Region { get; set; } = "";
        public bool IsCapital { get; set; }
        public bool IsCity { get; set; }
        public bool IsHarbor { get; set; }
        public int VictoryPoints { get; set; }
    }
}