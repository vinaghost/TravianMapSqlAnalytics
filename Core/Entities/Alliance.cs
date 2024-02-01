using Microsoft.EntityFrameworkCore;

namespace Core.Entities
{
    [Index(nameof(Name))]
    public class Alliance
    {
        public int Id { get; set; }

        public ICollection<Player> Players { get; set; }

        public string Name { get; set; } = "";
    }
}