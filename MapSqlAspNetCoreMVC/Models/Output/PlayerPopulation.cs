namespace MapSqlAspNetCoreMVC.Models.Output
{
    public class PlayerPopulation
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string AllianceName { get; set; }
        public string Tribe { get; set; }
        public int VillageCount { get; set; }
        public List<int> Population { get; set; }
    }
}