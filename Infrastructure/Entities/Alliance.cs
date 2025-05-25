namespace Infrastructure.Entities
{
    public class Alliance
    {
        public int Id { get; set; }

        public ICollection<Player> Players { get; set; } = [];
        public ICollection<AllianceHistory> History { get; set; } = [];
        public string Name { get; set; } = "";
        public int PlayerCount { get; set; }
    }
}