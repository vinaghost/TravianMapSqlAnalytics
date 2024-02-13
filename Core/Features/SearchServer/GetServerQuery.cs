﻿using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Core.Features.Shared.Query;

namespace Core.Features.SearchServer
{
    public record GetServerQuery(SearchParameters Parameters) : ICachedQuery<IList<SearchResult>>
    {
        public string CacheKey => $"{nameof(GetServerQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }
}