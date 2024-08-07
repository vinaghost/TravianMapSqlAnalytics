﻿namespace Features.Shared.Query
{
    public interface ICachedQuery<out TResponse> : IQuery<TResponse>, ICachedQuery;

    public interface ICachedQuery
    {
        string CacheKey { get; }
        TimeSpan? Expiation { get; }

        bool IsServerBased { get; }
    }
}