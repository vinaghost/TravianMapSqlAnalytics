using Application.Services;
using FastEndpoints;
using Features.Servers;
using FluentValidation.Results;
using MediatR;
using WebAPI.Contracts.Requests;

namespace WebAPI.Middlewares
{
    public class ServerMiddleware(DataService dataService, IMediator mediator) : IMiddleware
    {
        private readonly DataService _dataService = dataService;
        private readonly IMediator _mediator = mediator;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = context.GetRouteData().Values;
            if (value is not null && value.ContainsKey(nameof(IServerUrlRequest.ServerUrl)))
            {
                var server = value[nameof(IServerUrlRequest.ServerUrl)];
                if (server is not null)
                {
                    var serverUrl = server.ToString() ?? "";

                    var isValid = await _mediator.Send(new ValidateServerUrlQuery(serverUrl));
                    if (!isValid)
                    {
                        await context.Response.SendErrorsAsync([new ValidationFailure("Server", $"Server {serverUrl} is not available in database")]);
                        return;
                    }
                    _dataService.Server = serverUrl;
                }
            }

            await next(context);
        }
    }
}