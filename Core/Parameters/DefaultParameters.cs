namespace Core.Parameters
{
    public static class DefaultParameters
    {
        public static int MinPopulation => 0;
        public static int MaxPopulation => 10000;
        public static DateOnly Date => DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
        public static int MinChangePopulation => 0;
        public static int MaxChangePopulation => 10000;

        public static int MinChangeAlliance => 1;
        public static int MaxChangeAlliance => 10000;
        public static int PageNumber => 1;
        public static int PageSize => 50;

        public static int TargetX => 0;
        public static int TargetY => 0;

        public static int MinDistance => 0;
        public static int MaxDistance => 400;

        public static int Tribe => 0;
        public static bool IgnoreCapital => false;
        public static bool IgnoreNormalVillage => false;
    }
}