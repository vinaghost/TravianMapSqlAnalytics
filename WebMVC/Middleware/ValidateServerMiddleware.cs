using Core.Features.GetServer;
using Core.Services;
using MediatR;

namespace WebMVC.Middleware
{
    public class ValidateServerMiddleware(IMediator mediator, DataService dataService) : IMiddleware
    {
        private readonly IMediator _mediator = mediator;
        private readonly DataService _dataService = dataService;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var server = context.Request.Cookies["TMA_server"];
            if (string.IsNullOrEmpty(server))
            {
                server = await _mediator.Send(new GetMostPlayerServerQuery());
            }

            _dataService.Server = server;

            var options = new CookieOptions()
            {
                Expires = new DateTimeOffset(DateTime.Now.AddYears(1)),
            };
            context.Response.Cookies.Append("TMA_server", server, options);

            await next(context);
        }
    }
}