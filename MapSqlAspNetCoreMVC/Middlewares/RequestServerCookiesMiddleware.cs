using MapSqlAspNetCoreMVC.CQRS.Queries;
using MediatR;

namespace MapSqlAspNetCoreMVC.Middlewares
{
    public class RequestServerCookiesMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = context.Request.Query;
            if (value.ContainsKey("server"))
            {
                var server = value["server"].ToString();
                context.Response
                    .Cookies
                    .Append("server", server);

                context.Items["server"] = server;
            }

            await next(context);
        }
    }

    public class RequestServerMiddleware : IMiddleware
    {
        private readonly IMediator _mediator;

        public RequestServerMiddleware(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!string.IsNullOrEmpty(context.Items["server"] as string))
            {
                await next(context);
                return;
            }
            var server = context.Request.Cookies["server"];

            if (string.IsNullOrEmpty(server))
            {
                server = await _mediator.Send(new GetMostPlayerServerQuery());
            }

            context.Items["server"] = server;
            await next(context);
        }
    }

    public static class RequestServerCookiesMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestServerCookies(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestServerCookiesMiddleware>();
            app.UseMiddleware<RequestServerMiddleware>();
            return app;
        }
    }
}