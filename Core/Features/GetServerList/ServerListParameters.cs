namespace Core.Features.GetServerList
{
    public record ServerListParameters(string SearchTerm)
    {
        public string Key => $"{SearchTerm}";
    }
}