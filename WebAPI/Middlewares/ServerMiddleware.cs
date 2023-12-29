using MediatR;
using WebAPI.Queries;
using WebAPI.Services;

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
                var isValid = await _mediator.Send(new ValidateServerQuery(server));
                if (!isValid)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }
                _dataService.Server = server;
            }

            await next(context);
        }
    }
}