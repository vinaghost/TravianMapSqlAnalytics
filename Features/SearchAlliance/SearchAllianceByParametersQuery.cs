using Features.Shared.Models;
using Features.Shared.Parameters;
using Features.Shared.Query;
using X.PagedList;

namespace Features.SearchAlliance
{
    public record SearchAllianceByParametersQuery(NameFilterParameters Parameters) : ICachedQuery<IPagedList<SearchResult>>
    {
        public string CacheKey => $"{nameof(SearchAllianceByParametersQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}