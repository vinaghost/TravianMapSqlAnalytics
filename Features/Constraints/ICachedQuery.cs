namespace Features.Constraints
{
    public interface ICachedQuery : IQuery
    {
        string CacheKey { get; }
        TimeSpan? Expiration { get; }

        bool IsServerBased { get; }
    }

    public record DefaultCachedQuery(string CacheKey, bool IsServerBased = true) : ICachedQuery
    {
        public TimeSpan? Expiration => null;
    }
}