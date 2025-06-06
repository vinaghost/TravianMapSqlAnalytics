using Features.Servers;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class ServersController : Controller
    {
        private readonly IServerCache _serverCache;

        public ServersController(IServerCache serverCache)
        {
            _serverCache = serverCache;
        }

        public async Task<IActionResult> Change(
            string server,
            [FromServices] IsValidServerUrlQuery.Handler isValidServerUrlQuery
            )
        {
            var isValid = await isValidServerUrlQuery.HandleAsync(new(server));
            if (isValid)
            {
                _serverCache.Server = server;

                var options = new CookieOptions()
                {
                    Expires = new DateTimeOffset(DateTime.Now.AddYears(1)),
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                };

                Response.Cookies.Append("TMSA_server", server, options);
                return Ok();
            }
            return NotFound();
        }

        public async Task<IActionResult> Index(
            [FromServices] GetServersByNameQuery.Handler getServersByNameQuery,
            GetServersByNameParameters parameters
        )
        {
            var servers = await getServersByNameQuery.HandleAsync(new(parameters));
            return Json(new { results = servers.Select(x => new { Id = x.Url, Text = x.Url }), pagination = new { more = servers.PageNumber * servers.PageSize < servers.TotalItemCount } });
        }
    }
}