namespace MainCore.Models
{
    public class PlayerAlliance
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public DateTime Date { get; set; }

        public int AllianceId { get; set; }
    }
}