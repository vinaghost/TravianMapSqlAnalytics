using Features.Shared.Models;
using Features.Shared.Query;

namespace Features.SearchAlliance
{
    public record SearchAllianceByIdQuery(IList<int> Ids) : ICachedQuery<IList<SearchResult>>
    {
        public string CacheKey => $"{nameof(SearchAllianceByIdQuery)}_{string.Join(',', Ids.Distinct().Order())}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}