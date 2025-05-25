namespace Infrastructure.Entities
{
    public class AllianceHistory
    {
        public int Id { get; set; }

        public int AllianceId { get; set; }

        public DateTime Date { get; set; }

        public int PlayerCount { get; set; }
        public bool ChangePlayerCount { get; set; }
    }
}