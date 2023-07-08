namespace MainCore.Models
{
    public class Player
    {
        public Player(VillageRaw village)
        {
            Id = village.PlayerId;
            Name = village.PlayerName;
            AllianceId = village.AllyId;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int AllianceId { get; set; }
    }
}