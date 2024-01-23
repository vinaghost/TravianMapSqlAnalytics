using Core.Features.GetServer;
using Core.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class ServersController(IMediator mediator, DataService dataService) : Controller
    {
        private readonly IMediator _mediator = mediator;
        private readonly DataService _dataService = dataService;

        public IActionResult Index(ServerParameters parameters)
        {
            return View(parameters);
        }

        public async Task<IActionResult> Change(string server)
        {
            var isValid = await _mediator.Send(new ValidateServerQuery(server));
            if (isValid)
            {
                _dataService.Server = server;

                var options = new CookieOptions()
                {
                    Expires = new DateTimeOffset(DateTime.Now.AddYears(1)),
                };
                Response.Cookies.Append("TMA_server", server, options);

                var cookie = options.CreateCookieHeader("TMA_server", server);
                return Ok(cookie.ToString());
            }
            return NotFound();
        }
    }
}