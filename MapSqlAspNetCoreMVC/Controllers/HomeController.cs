using MapSqlAspNetCoreMVC.CQRS.Queries;
using MapSqlAspNetCoreMVC.Models;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.View;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MapSqlAspNetCoreMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IMediator mediator)
        {
            _logger = logger;
            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(HomeInput input)
        {
            input ??= new HomeInput();

            var viewModel = new HomeViewModel()
            {
                Input = input,
                Servers = await _mediator.Send(new GetServersQuery(input)),
                ServerUrl = HttpContext.Items["server"] as string,
                Today = await _mediator.Send(new GetNewestDayQuery()),
            };
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}