namespace Features.Shared.Parameters
{
    public static class DefaultParameters
    {
        public const int MinPopulation = 0;
        public const int MaxPopulation = 10000;
        public static readonly DateTime Date = DateTime.Today.AddDays(-7);
        public const int MinChangePopulation = 0;
        public const int MaxChangePopulation = 10000;

        public const int MinChangeAlliance = 1;
        public const int MaxChangeAlliance = 10000;
        public const int PageNumber = 1;
        public const int PageSize = 50;

        public const int X = 0;
        public const int Y = 0;

        public const int MinDistance = 0;
        public const int MaxDistance = 400;

        public const int Tribe = 0;
        public const bool IgnoreCapital = false;
        public const bool IgnoreNormalVillage = false;

        // 0 asc / 1 desc
        public const int SortOrder = 0;

        public const string SortField = "#";
    }
}