namespace MapSqlAspNetCoreMVC.Middlewares
{
    public class RequestServerCookiesMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = context.Request.Query;
            if (value.ContainsKey("server"))
            {
                var server = value["server"];
                context.Response
                    .Cookies
                    .Append("server", server);
            }

            await next(context);
        }
    }

    public static class RequestServerCookiesMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestServerCookies(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestServerCookiesMiddleware>();
            return app;
        }
    }
}