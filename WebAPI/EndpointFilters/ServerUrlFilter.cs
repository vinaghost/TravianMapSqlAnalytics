using Application.Services;
using Features.Servers;
using MediatR;
using WebAPI.Contracts.Requests;

namespace WebAPI.EndpointFilters
{
    public class ServerUrlFilter(DataService dataService, IMediator mediator) : IEndpointFilter
    {
        private readonly DataService _dataService = dataService;
        private readonly IMediator _mediator = mediator;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var value = context.HttpContext.GetRouteData().Values;
            if (value is not null && value.ContainsKey(nameof(IServerUrlRequest.ServerUrl)))
            {
                var server = value[nameof(IServerUrlRequest.ServerUrl)];
                if (server is not null)
                {
                    var serverUrl = server.ToString() ?? "";

                    var isValid = await _mediator.Send(new ValidateServerUrlQuery(serverUrl));
                    if (!isValid)
                    {
                        return Results.Problem($"Server {serverUrl} is not available in database", statusCode: 404);
                    }
                    _dataService.Server = serverUrl;
                }
            }

            return await next(context);
        }
    }
}