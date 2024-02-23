namespace Features.Shared.Parameters
{
    public static class DefaultParameters
    {
        public static readonly DateTime Date = DateTime.Today.AddDays(-7);

        public const int PageNumber = 1;
        public const int PageSize = 50;
    }
}