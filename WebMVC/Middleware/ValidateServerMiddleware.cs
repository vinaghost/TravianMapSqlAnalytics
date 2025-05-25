using Features.Servers;
using Infrastructure.Services;
using MediatR;

namespace WebMVC.Middleware
{
    public class ValidateServerMiddleware(IMediator mediator, IServerCache serverCache) : IMiddleware
    {
        private readonly IMediator _mediator = mediator;
        private readonly IServerCache _serverCache = serverCache;

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
                var valid = await _mediator.Send(new IsValidServerUrlQuery(server));
                if (!valid)
                {
                    server = await _mediator.Send(new GetMostPlayerServerQuery());
                    SetCookie(context, server);
                }
            }

            _serverCache.Server = server;

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