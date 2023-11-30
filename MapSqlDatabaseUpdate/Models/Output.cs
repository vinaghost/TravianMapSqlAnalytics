namespace MapSqlDatabaseUpdate.Models
{
    public class Output
    {
        public string ServerUrl { get; set; }
        public int AllianceCount { get; set; }
        public int PlayerCount { get; set; }
        public int VillageCount { get; set; }

        public override string ToString()
        {
            return $"{ServerUrl} - {AllianceCount} alliances - {PlayerCount} players - {VillageCount} villages";
        }
    }
}