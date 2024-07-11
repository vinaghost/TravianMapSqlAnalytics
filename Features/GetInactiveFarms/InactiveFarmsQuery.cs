using Features.Shared.Dtos;
using Features.Shared.Query;
using X.PagedList;

namespace Features.GetInactiveFarms
{
    public record InactiveFarmsQuery(InactiveFarmParameters Parameters) : ICachedQuery<IPagedList<VillageDataDto>>
    {
        public string CacheKey => $"{nameof(InactiveFarmsQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}