using Microsoft.EntityFrameworkCore;

namespace Core.Entities
{
    [Index(nameof(VillageId), nameof(Date))]
    public class VillagePopulationHistory
    {
        public int Id { get; set; }

        public int VillageId { get; set; }

        public DateTime Date { get; set; }

        public int Population { get; set; }

        public int Change { get; set; }
    }
}