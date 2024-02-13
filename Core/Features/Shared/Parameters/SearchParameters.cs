namespace Core.Features.Shared.Parameters
{
    public record SearchParameters(string SearchTerm)
    {
        public string Key => $"{SearchTerm}";
    }
}