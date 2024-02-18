﻿using Core.Features.Shared.Query;
using X.PagedList;

namespace Core.Features.GetInactiveFarms
{
    public record GetInactiveFarmsQuery(InactiveFarmParameters Parameters) : ICachedQuery<IPagedList<InactiveFarmDto>>
    {
        public string CacheKey => $"{nameof(GetInactiveFarmsQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => TimeSpan.FromMilliseconds(1);

        public bool IsServerBased => true;
    }
}