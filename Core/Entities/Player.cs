using Microsoft.EntityFrameworkCore;

namespace Core.Entities
{
    [Index(nameof(Name))]
    public class Player
    {
        public int Id { get; set; }
        public ICollection<Village> Villages { get; set; }

        public ICollection<PlayerAllianceHistory> Alliances { get; set; }

        public int AllianceId { get; set; }

        public string Name { get; set; } = "";
    }
}