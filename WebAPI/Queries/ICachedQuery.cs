namespace WebAPI.Queries
{
    public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;

    public interface ICachedQuery
    {
        string CacheKey { get; }
        TimeSpan? Expiation { get; }

        bool IsServerBased { get; }
    }
}