using Core;
using Microsoft.EntityFrameworkCore;
using WebAPI.Services;

namespace WebAPI.Middlewares
{
    public class ServerMiddleware(DataService dataService, ServerListDbContext serverListDbContext) : IMiddleware
    {
        private readonly DataService _dataService = dataService;
        private readonly ServerListDbContext _serverListContext = serverListDbContext;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = context.Request.Query;
            if (value.ContainsKey("server"))
            {
                var server = value["server"].ToString();
                if (!await ValidateServer(server))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }
                _dataService.Server = server;
            }

            await next(context);
        }

        private async Task<bool> ValidateServer(string server)
        {
            return await _serverListContext.Servers
                .Where(x => x.Url == server)
                .AnyAsync();
        }
    }
}