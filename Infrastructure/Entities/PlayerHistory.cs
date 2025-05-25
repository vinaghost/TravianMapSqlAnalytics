namespace Infrastructure.Entities
{
    public class PlayerHistory
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public DateTime Date { get; set; }

        public int AllianceId { get; set; }

        public bool ChangeAlliance { get; set; }

        public int Population { get; set; }

        public int ChangePopulation { get; set; }
    }
}