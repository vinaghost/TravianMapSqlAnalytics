using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    [Index(nameof(Name))]
    public class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public ICollection<Village> Villages { get; set; }

        public ICollection<PlayerAllianceHistory> Alliances { get; set; }

        public int AllianceId { get; set; }

        public string Name { get; set; } = "";
    }
}