namespace MapSqlQuery.Models.Database
{
    public class VillagePopulation
    {
        // primary key
        public int Id { get; set; }

        // properties

        public int VillageId { get; set; }

        public DateTime Date { get; set; }

        public int Population { get; set; }
    }
}