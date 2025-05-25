using Features.Queries.Servers;
using Infrastructure.Services;

namespace WebMVC.Middleware
{
    public class ValidateServerMiddleware : IMiddleware
    {
        private readonly IServerCache _serverCache;
        private readonly GetMostPlayerServerQuery.Handler _getMostPlayerServerQuery;
        private readonly IsValidServerUrlQuery.Handler _isValidServerUrlQuery;

        public ValidateServerMiddleware(IServerCache serverCache, GetMostPlayerServerQuery.Handler getMostPlayerServerQuery, IsValidServerUrlQuery.Handler isValidServerUrlQuery)
        {
            _serverCache = serverCache;
            _getMostPlayerServerQuery = getMostPlayerServerQuery;
            _isValidServerUrlQuery = isValidServerUrlQuery;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var server = context.Request.Cookies["TMSA_server"];
            if (string.IsNullOrEmpty(server))
            {
                server = await _getMostPlayerServerQuery.HandleAsync(new());
                SetCookie(context, server);
            }
            else
            {
                var valid = await _isValidServerUrlQuery.HandleAsync(new(server));
                if (!valid)
                {
                    server = await _getMostPlayerServerQuery.HandleAsync(new());
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