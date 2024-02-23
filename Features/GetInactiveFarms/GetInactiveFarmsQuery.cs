using Features.Shared.Dtos;
using Features.Shared.Query;
using X.PagedList;

namespace Features.GetInactiveFarms
{
    public record GetInactiveFarmsQuery(InactiveFarmParameters Parameters) : ICachedQuery<IPagedList<VillageDataDto>>
    {
        public string CacheKey => $"{nameof(GetInactiveFarmsQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => TimeSpan.FromMilliseconds(1);

        public bool IsServerBased => true;
    }
}