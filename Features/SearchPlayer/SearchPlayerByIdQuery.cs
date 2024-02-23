using Features.Shared.Models;
using Features.Shared.Query;

namespace Features.SearchPlayer
{
    public record SearchPlayerByIdQuery(IList<int> Ids) : ICachedQuery<IList<SearchResult>>
    {
        public string CacheKey => $"{nameof(SearchPlayerByIdQuery)}_{string.Join(',', Ids.Distinct().Order())}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}