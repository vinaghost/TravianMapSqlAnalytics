using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Core.Features.Shared.Query;

namespace Core.Features.SearchAlliance
{
    public record GetAllianceQuery(SearchParameters Parameters) : ICachedQuery<IList<SearchResult>>
    {
        public string CacheKey => $"{nameof(GetAllianceQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}