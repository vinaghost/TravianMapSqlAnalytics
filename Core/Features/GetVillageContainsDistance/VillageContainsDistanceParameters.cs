using Core.Features.Shared.Parameters;

namespace Core.Features.GetVillageContainsDistance
{
    public record VillageContainsDistanceParameters : IPaginationParameters, IVillageFilterParameters, IDistanceFilterParameters, ISortParameters
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
        public virtual string Key => $"{PageNumber}_{PageSize}_{TargetX}_{TargetY}_{MinDistance}_{MaxDistance}_{SortOrder}_{SortField}_{MinPopulation}_{MaxPopulation}_{Tribe}_{IgnoreCapital}_{IgnoreNormalVillage}_{string.Join(',', Alliances)}_{string.Join(',', Players)}";

        public int TargetX { get; set; } = DefaultParameters.TargetX;
        public int TargetY { get; set; } = DefaultParameters.TargetY;
        public int MinDistance { get; set; } = DefaultParameters.MinDistance;
        public int MaxDistance { get; set; } = DefaultParameters.MaxDistance;

        public int SortOrder { get; set; } = DefaultParameters.SortOrder;

        public string SortField { get; set; } = DefaultParameters.SortField;
    }
}