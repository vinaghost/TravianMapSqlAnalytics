namespace WebAPI.Models.Parameters
{
    public record VillageParameters : IPaginationParameters, IVillageFilterParameter
    {
        public int PageNumber { get; } = DefaultParameters.PageNumber;
        public int PageSize { get; } = DefaultParameters.PageSize;
        public int MinPopulation { get; } = DefaultParameters.MinPopulation;
        public int MaxPopulation { get; } = DefaultParameters.MaxPopulation;
        public List<int> Alliances { get; } = [];
        public List<int> Players { get; } = [];
        public List<int> Villages { get; } = [];

        public void Deconstruct(
            out List<int> alliances,
            out List<int> players,
            out List<int> villages,
            out int minPopulation,
            out int maxPopulation
        ) => (alliances, players, villages, minPopulation, maxPopulation) = (Alliances, Players, Villages, MinPopulation, MaxPopulation);
    }
}