using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Core.Features.Shared.Query;
using X.PagedList;

namespace Core.Features.SearchPlayer
{
    public record SearchPlayerByParametersQuery(SearchParameters Parameters) : ICachedQuery<IPagedList<SearchResult>>
    {
        public string CacheKey => $"{nameof(SearchPlayerByParametersQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}