using Features.Shared.Dtos;
using Features.Shared.Query;
using X.PagedList;

namespace Features.GetNeighbors
{
    public record GetNeighborsQuery(NeighborsParameters Parameters) : ICachedQuery<IPagedList<VillageDataDto>>
    {
        public string CacheKey => $"{nameof(GetNeighborsQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => TimeSpan.FromMilliseconds(1);

        public bool IsServerBased => true;
    }
}