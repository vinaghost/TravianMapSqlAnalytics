using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Entities
{
    [Index(nameof(VillageId), nameof(Date))]
    [Index(nameof(VillageId), nameof(ChangePopulation))]
    public class VillageHistory
    {
        public int Id { get; set; }

        public int VillageId { get; set; }

        public DateTime Date { get; set; }

        public int Population { get; set; }

        public int ChangePopulation { get; set; }
    }
}