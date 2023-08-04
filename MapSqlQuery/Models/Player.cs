using System.ComponentModel.DataAnnotations.Schema;

namespace MapSqlQuery.Models
{
    public class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AllianceId { get; set; }

        public string Name { get; set; } = "";

        public List<Village> Villages { get; set; } = new();
        public List<VillagePopulation> Populations { get; set; } = new();
    }
}