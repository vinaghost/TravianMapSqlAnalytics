namespace Core.Parameters
{
    public record VillageContainsDistanceParameters : IPaginationParameters, IVillageFilterParameter
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;
        public int PageSize { get; set; } = DefaultParameters.PageSize;
        public int MinPopulation { get; set; } = DefaultParameters.MinPopulation;
        public int MaxPopulation { get; set; } = DefaultParameters.MaxPopulation;
        public List<int> Alliances { get; set; } = [];
        public List<int> Players { get; set; } = [];
        public virtual string Key => $"{PageNumber}_{PageSize}_{MinPopulation}_{TargetX}_{TargetY}_{MinDistance}_{MaxDistance}_{MaxPopulation}_{string.Join(',', Alliances)}_{string.Join(',', Players)}";

        public int TargetX { get; set; } = DefaultParameters.TargetX;
        public int TargetY { get; set; } = DefaultParameters.TargetY;
        public int MinDistance { get; set; } = DefaultParameters.MinDistance;
        public int MaxDistance { get; set; } = DefaultParameters.MaxDistance;
    }
}