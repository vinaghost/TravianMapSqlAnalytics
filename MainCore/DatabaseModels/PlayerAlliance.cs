namespace MainCore.DatabaseModels
{
    public class PlayerAlliance
    {
        // primary key
        public int Id { get; set; }

        // properties
        public int PlayerId { get; set; }

        public DateTime Date { get; set; }

        public int AllianceId { get; set; }
    }
}