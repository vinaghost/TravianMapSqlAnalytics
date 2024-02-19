using Core.Features.Shared.Dtos;
using Core.Features.Shared.Query;
using X.PagedList;

namespace Core.Features.GetNeighbors
{
    public record GetNeighborsQuery(NeighborsParameters Parameters) : ICachedQuery<IPagedList<VillageDataDto>>
    {
        public string CacheKey => $"{nameof(GetNeighborsQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => TimeSpan.FromMilliseconds(1);

        public bool IsServerBased => true;
    }
}