namespace Core.Parameters
{
    public record VillageContainsPopulationHistoryParameters : IPaginationParameters, IVillageFilterParameter, ISortParameters
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;
        public int PageSize { get; set; } = DefaultParameters.PageSize;
        public List<int> Alliances { get; set; } = [];
        public List<int> Players { get; set; } = [];

        public int MinPopulation { get; set; } = DefaultParameters.MinPopulation;
        public int MaxPopulation { get; set; } = DefaultParameters.MaxPopulation;

        public int Tribe { get; set; } = DefaultParameters.Tribe;

        public bool IgnoreCapital { get; set; } = DefaultParameters.IgnoreCapital;

        public bool IgnoreNormalVillage { get; set; } = DefaultParameters.IgnoreNormalVillage;
        public string Key => $"{PageNumber}_{PageSize}_{Tribe}_{IgnoreCapital}_{IgnoreNormalVillage}_{MinPopulation}_{MaxPopulation}_{string.Join(',', Alliances)}_{string.Join(',', Players)}_{MinChangePopulation}_{MaxChangePopulation}_{Date}_{SortOrder}_{SortField}";

        public int MinChangePopulation { get; set; } = DefaultParameters.MinChangePopulation;
        public int MaxChangePopulation { get; set; } = DefaultParameters.MaxChangePopulation;
        public DateOnly Date { get; set; } = DefaultParameters.Date;

        public int SortOrder { get; set; } = DefaultParameters.SortOrder;

        public string SortField { get; set; } = DefaultParameters.SortField;
    }
}