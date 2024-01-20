using Core.Features.Shared.Query;
using Core.Services;
using MediatR;

namespace Core.Behaviors
{
    public sealed class QueryCachingPipelineBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICachedQuery
    {
        private readonly ICacheService _cacheService;
        private readonly DataService _dataService;

        public QueryCachingPipelineBehavior(ICacheService cacheService, DataService dataService)
        {
            _cacheService = cacheService;
            _dataService = dataService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheKey = request.CacheKey;

            if (request.IsServerBased)
            {
                cacheKey = $"{_dataService.Server}_{cacheKey}";
            }

            return await _cacheService.GetOrCreateAsync(
                cacheKey,
                _ => next(),
                request.Expiation,
                cancellationToken);
        }
    }
}