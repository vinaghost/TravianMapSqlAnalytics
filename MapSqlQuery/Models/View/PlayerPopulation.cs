namespace MapSqlQuery.Models.View
{
    public class PlayerPopulation
    {
        public PlayerPopulation()
        {
            Population = new();
        }

        public int PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public string? AllianceName { get; set; }
        public int TribeId { get; set; }

        public string Tribe
        {
            get
            {
                return TribeId switch
                {
                    1 => "Romans",
                    2 => "Teutons",
                    3 => "Gauls",
                    4 => "Nature",
                    5 => "Natars",
                    6 => "Egyptians",
                    7 => "Huns",
                    8 => "Spartans",
                    _ => "",
                };
            }
        }

        public int VillageCount { get; set; }
        public List<int> Population { get; set; }
        public int PopulationChange { get; set; }
    }
}