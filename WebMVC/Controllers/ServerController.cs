using Features.Servers;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class ServersController(IMediator mediator, IServerCache serverCache) : Controller
    {
        private readonly IMediator _mediator = mediator;
        private readonly IServerCache _serverCache = serverCache;

        public async Task<IActionResult> Change(string server)
        {
            var isValid = await _mediator.Send(new IsValidServerUrlQuery(server));
            if (isValid)
            {
                _serverCache.Server = server;

                var options = new CookieOptions()
                {
                    Expires = new DateTimeOffset(DateTime.Now.AddYears(1)),
                    SameSite = SameSiteMode.Lax
                };

                Response.Cookies.Append("TMSA_server", server, options);
                return Ok();
            }
            return NotFound();
        }

        public async Task<IActionResult> Index(GetServersByNameParameters parameters)
        {
            var servers = await _mediator.Send(new GetServersByNameQuery(parameters));
            return Json(new { results = servers.Select(x => new { Id = x.Url, Text = x.Url }), pagination = new { more = servers.PageNumber * servers.PageSize < servers.TotalItemCount } });
        }
    }
}