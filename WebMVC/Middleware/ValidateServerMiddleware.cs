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
            var server = context.Request.Cookies["TMSA_server"];
            if (string.IsNullOrEmpty(server))
            {
                server = await _mediator.Send(new GetMostPlayerServerQuery());
                SetCookie(context, server);
            }
            else
            {
                var valid = await _mediator.Send(new ValidateServerUrlQuery(server));
                if (!valid)
                {
                    server = await _mediator.Send(new GetMostPlayerServerQuery());
                    SetCookie(context, server);
                }
            }

            _dataService.Server = server;

            await next(context);
        }

        private void SetCookie(HttpContext context, string server)
        {
            var options = new CookieOptions()
            {
                Expires = new DateTimeOffset(DateTime.Now.AddYears(1)),
            };
            context.Response.Cookies.Append("TMSA_server", server, options);
        }
    }
}