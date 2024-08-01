namespace Features.Shared.Parameters
{
    public record NameFilterParameters : IPaginationParameters
    {
        public string? Name { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
    }

    public static class SearchParametersExtension
    {
        public static string Key(this NameFilterParameters parameters)
        {
            return $"{parameters.Name}_{parameters.PageNumber}_{parameters.PageSize}";
        }
    }
}