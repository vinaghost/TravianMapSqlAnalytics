using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    [Index(nameof(Name))]
    public class Alliance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public ICollection<Player> Players { get; set; }

        public string Name { get; set; } = "";
    }
}