using Application.Services;
using Features.Shared.Query;
using MediatR;

namespace Features.Shared.Behaviors
{
    public sealed class QueryCachingPipelineBehavior<TRequest, TResponse>(CacheService cacheService, DataService dataService)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICachedQuery
    {
        private readonly CacheService _cacheService = cacheService;
        private readonly DataService _dataService = dataService;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheKey = request.CacheKey;

            if (request.IsServerBased)
            {
                cacheKey = $"{_dataService.Server}_{cacheKey}";
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