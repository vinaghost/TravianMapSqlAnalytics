using Features.Constraints;
using Features.Services;
using Immediate.Handlers.Shared;
using Infrastructure.Services;

namespace Features.Behaviors
{
    public sealed class QueryCachingBehavior<TRequest, TResponse>
        : Behavior<TRequest, TResponse>
        where TRequest : ICachedQuery
    {
        private readonly CacheService _cacheService;
        private readonly IServerCache _serverCache;

        public QueryCachingBehavior(CacheService cacheService, IServerCache serverCache)
        {
            _cacheService = cacheService;
            _serverCache = serverCache;
        }

        public override async ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            var cacheKey = request.CacheKey;
            if (request.IsServerBased)
            {
                cacheKey = $"{_serverCache.Server}_{cacheKey}";
            }

            var cachedValue = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async token => await Next(request, token),
                request.Expiration,
                cancellationToken);

            return cachedValue!;
        }
    }
}