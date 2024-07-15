using Features.Shared.Dtos;
using Features.Shared.Query;
using X.PagedList;

namespace Features.GetInactiveFarms
{
    public record GetInactiveQuery(InactiveParameters Parameters) : ICachedQuery<IPagedList<VillageDataDto>>
    {
        public string CacheKey => $"{nameof(GetInactiveQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}