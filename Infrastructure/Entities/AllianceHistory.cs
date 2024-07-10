using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Entities
{
    [Index(nameof(AllianceId), nameof(Date))]
    [Index(nameof(AllianceId), nameof(ChangePlayerCount))]
    public class AllianceHistory
    {
        public int Id { get; set; }

        public int AllianceId { get; set; }

        public DateTime Date { get; set; }

        public int PlayerCount { get; set; }
        public bool ChangePlayerCount { get; set; }
    }
}