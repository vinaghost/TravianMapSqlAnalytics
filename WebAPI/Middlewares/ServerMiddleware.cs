using WebAPI.Services;

namespace WebAPI.Middlewares
{
    public class ServerMiddleware(DataService dataService) : IMiddleware
    {
        private readonly DataService _dataService = dataService;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = context.Request.Query;
            if (value.ContainsKey("server"))
            {
                var server = value["server"].ToString();
                _dataService.Server = server;
            }

            await next(context);
        }
    }
}