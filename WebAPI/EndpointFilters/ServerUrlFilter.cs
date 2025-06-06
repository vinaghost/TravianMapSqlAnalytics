using Features.Servers;
using Infrastructure.Services;
using WebAPI.Contracts.Requests;

namespace WebAPI.EndpointFilters
{
    public class ServerUrlFilter : IEndpointFilter
    {
        private readonly IServerCache _serverCache;
        private readonly IsValidServerUrlQuery.Handler _isValidServerUrlQuery;

        public ServerUrlFilter(IServerCache serverCache, IsValidServerUrlQuery.Handler isValidServerUrlQuery)
        {
            _serverCache = serverCache;
            _isValidServerUrlQuery = isValidServerUrlQuery;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var value = context.HttpContext.GetRouteData().Values;
            if (value is not null && value.ContainsKey(nameof(IServerUrlRequest.ServerUrl)))
            {
                var server = value[nameof(IServerUrlRequest.ServerUrl)];
                if (server is not null)
                {
                    var serverUrl = server.ToString() ?? "";

                    var isValid = await _isValidServerUrlQuery.HandleAsync(new(serverUrl));
                    if (!isValid)
                    {
                        return Results.Problem($"Server {serverUrl} is not available in database", statusCode: 404);
                    }
                    _serverCache.Server = serverUrl;
                }
            }

            return await next(context);
        }
    }
}