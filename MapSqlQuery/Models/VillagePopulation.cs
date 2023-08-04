using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapSqlQuery.Models
{
    [PrimaryKey(nameof(VillageId), nameof(Date))]
    public class VillagePopulation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillageId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public DateTime Date { get; set; }

        public int Population { get; set; }

        public int PlayerId { get; set; }
    }
}