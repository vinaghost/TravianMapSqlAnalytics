namespace Infrastructure.Entities
{
    public class Village
    {
        public int Id { get; set; }

        public ICollection<VillageHistory> History { get; set; } = [];

        public int MapId { get; set; }

        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public int Tribe { get; set; }
        public int Population { get; set; }
        public string Region { get; set; } = "";
        public bool IsCapital { get; set; }
        public bool IsCity { get; set; }
        public bool IsHarbor { get; set; }
        public int VictoryPoints { get; set; }
    }
}