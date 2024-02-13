using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Core.Features.Shared.Query;

namespace Core.Features.SearchPlayer
{
    public record GetPlayerQuery(SearchParameters Parameters) : ICachedQuery<IList<SearchResult>>
    {
        public string CacheKey => $"{nameof(GetPlayerQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}