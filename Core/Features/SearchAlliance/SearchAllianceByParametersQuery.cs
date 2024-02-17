using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Core.Features.Shared.Query;
using X.PagedList;

namespace Core.Features.SearchAlliance
{
    public record SearchAllianceByParametersQuery(SearchParameters Parameters) : ICachedQuery<IPagedList<SearchResult>>
    {
        public string CacheKey => $"{nameof(SearchAllianceByParametersQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}