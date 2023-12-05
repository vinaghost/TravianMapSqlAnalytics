using MapSqlAspNetCoreMVC.CQRS.Queries;
using MapSqlAspNetCoreMVC.Models;
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

        public async Task<IActionResult> Index()
        {
            ViewBag.NewestDateTime = (await _mediator.Send(new GetNewestDayQuery())).ToString("yyyy-MM-dd");
            ViewData["ServerInfo"] = HttpContext.Items["server"] as string;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}