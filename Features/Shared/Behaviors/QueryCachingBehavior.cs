using Features.Shared.Constraints;
using Features.Shared.Services;

using Immediate.Handlers.Shared;

namespace Features.Shared.Behaviors
{
    public sealed class QueryCachingBehavior<TRequest, TResponse>
        : Behavior<TRequest, TResponse>
        where TRequest : ICachedQuery
    {
        private readonly CacheService _cacheService;
        private readonly ServerCache _serverCache;

        public QueryCachingBehavior(CacheService cacheService, ServerCache serverCache)
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