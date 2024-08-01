using Features.Shared.Models;
using Features.Shared.Parameters;
using Features.Shared.Query;
using X.PagedList;

namespace Features.SearchPlayer
{
    public record SearchPlayerByParametersQuery(NameFilterParameters Parameters) : ICachedQuery<IPagedList<SearchResult>>
    {
        public string CacheKey => $"{nameof(SearchPlayerByParametersQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}