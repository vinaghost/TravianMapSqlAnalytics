using Features.Shared.Models;
using Features.Shared.Query;

namespace Features.SearchPlayer
{
    public record SearchPlayerByAllianceIdQuery(int AllianceId) : ICachedQuery<IList<SearchResult>>
    {
        public string CacheKey => $"{nameof(SearchPlayerByAllianceIdQuery)}_{AllianceId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}