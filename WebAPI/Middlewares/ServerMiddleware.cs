using Application.Services;
using FastEndpoints;
using Features.GetServer;
using FluentValidation.Results;
using MediatR;

namespace WebAPI.Middlewares
{
    public class ServerMiddleware(DataService dataService, IMediator mediator) : IMiddleware
    {
        private readonly DataService _dataService = dataService;
        private readonly IMediator _mediator = mediator;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = context.Request.Query;
            if (value.ContainsKey("server"))
            {
                var server = value["server"].ToString();
                var isValid = await _mediator.Send(new ValidateServerUrlQuery(server));
                if (!isValid)
                {
                    await context.Response.SendErrorsAsync([new ValidationFailure("Server", "Server is not available in database")]);
                    return;
                }
                _dataService.Server = server;
            }

            await next(context);
        }
    }
}