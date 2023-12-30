namespace WebAPI.Models.Parameters
{
    public static class DefaultParameters
    {
        public static int MinPopulation => 0;
        public static int MaxPopulation => 10000;
        public static DateOnly Date => DateOnly.FromDateTime(DateTime.Now);
        public static int MinChangePopulation => 0;
        public static int MaxChangePopulation => 10000;
        public static int PageNumber => 1;
        public static int PageSize => 20;
    }
}