using Features.Shared.Models;
using Features.Shared.Parameters;
using Features.Shared.Query;
using X.PagedList;

namespace Features.SearchServer
{
    public record GetServerQuery(NameFilterParameters Parameters) : ICachedQuery<IPagedList<SearchResult>>
    {
        public string CacheKey => $"{nameof(GetServerQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }
}