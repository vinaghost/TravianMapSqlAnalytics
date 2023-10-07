using System.ComponentModel.DataAnnotations.Schema;

namespace MainCore.Models
{
    public class Alliance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AllianceId { get; set; }

        public string Name { get; set; } = "";
        public ICollection<Player> Players { get; set; }
    }
}