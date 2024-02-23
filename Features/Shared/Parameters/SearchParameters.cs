namespace Features.Shared.Parameters
{
    public record SearchParameters(string SearchTerm, int Page, int PageSize)
    {
        public string Key => $"{SearchTerm}_{Page}_{PageSize}";
    }
}