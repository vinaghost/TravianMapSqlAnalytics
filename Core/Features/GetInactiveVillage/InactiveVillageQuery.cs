using Core.Features.Shared.Query;
using X.PagedList;

namespace Core.Features.GetInactiveVillage
{
    public record InactiveVillageQuery(InactiveVillagesParameters Parameters) : ICachedQuery<IPagedList<InactiveVillageDto>>
    {
        public string CacheKey => $"{nameof(InactiveVillageQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}