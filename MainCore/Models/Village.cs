namespace MainCore.Models
{
    public class Village
    {
        public Village(VillageRaw village)
        {
            Id = village.Id;
            MapId = village.MapId;

            Name = village.Name;
            X = village.X;
            Y = village.Y;
            Tribe = village.Tribe;
            PlayerId = village.PlayerId;
            Pop = village.Pop;
            Region = village.Region;
            IsCapital = village.IsCapital;
            IsCity = village.IsCity;
            VictoryPoints = village.VictoryPoints;
        }

        public int Id { get; set; }
        public int MapId { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Tribe { get; set; }
        public int PlayerId { get; set; }
        public int Pop { get; set; }
        public string Region { get; set; }
        public bool IsCapital { get; set; }
        public bool IsCity { get; set; }
        public int VictoryPoints { get; set; }
    }
}