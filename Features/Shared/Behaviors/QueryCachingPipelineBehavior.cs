using Features.Services;
using Features.Shared.Query;
using Infrastructure.Services;

namespace Features.Shared.Behaviors
{
    public sealed class QueryCachingPipelineBehavior<TRequest, TResponse>(CacheService cacheService, IServerCache serverCache)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICachedQuery
    {
        private readonly CacheService _cacheService = cacheService;
        private readonly IServerCache _serverCache = serverCache;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheKey = request.CacheKey;

            if (request.IsServerBased)
            {
                cacheKey = $"{_serverCache.Server}_{cacheKey}";
            }

            var cachedValue = await _cacheService.GetOrCreateAsync(
                cacheKey,
                _ => next(),
                request.Expiation,
                cancellationToken);

            return cachedValue!;
        }
    }
}