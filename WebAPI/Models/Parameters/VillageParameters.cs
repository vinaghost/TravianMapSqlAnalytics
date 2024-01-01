namespace WebAPI.Models.Parameters
{
    public record VillageParameters : IPaginationParameters, IVillageFilterParameter
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;
        public int PageSize { get; set; } = DefaultParameters.PageSize;
        public int MinPopulation { get; set; } = DefaultParameters.MinPopulation;
        public int MaxPopulation { get; set; } = DefaultParameters.MaxPopulation;
        public List<int> Alliances { get; set; } = [];
        public List<int> Players { get; set; } = [];
        public List<int> Villages { get; set; } = [];
        public virtual string Key => $"{PageNumber}_{PageSize}_{MinPopulation}_{MaxPopulation}_{string.Join(',', Alliances)}_{string.Join(',', Players)}_{string.Join(',', Villages)}";
    }
}