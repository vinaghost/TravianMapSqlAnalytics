namespace WebAPI.Models.Parameters
{
    public record ChangePopulationVillageParameters : VillageParameters, IChangePopulationFilterParameter
    {
        public int MinChangePopulation { get; set; } = DefaultParameters.MinChangePopulation;
        public int MaxChangePopulation { get; set; } = DefaultParameters.MaxChangePopulation;
        public DateOnly Date { get; set; } = DefaultParameters.Date;

        public void Deconstruct(
            out DateOnly date,
            out List<int> alliances,
            out List<int> players,
            out List<int> villages,
            out int minPopulation,
            out int maxPopulation
        ) => (date, alliances, players, villages, minPopulation, maxPopulation) = (Date, Alliances, Players, Villages, MinPopulation, MaxPopulation);
    }
}