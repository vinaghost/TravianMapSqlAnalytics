using InactiveFinder.Models;
using InactiveFinder.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InactiveFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataProvider _dataProvider;

        private DateTime _newestDateTime;

        public HomeController(ILogger<HomeController> logger, DataProvider dataProvider)
        {
            _logger = logger;
            _dataProvider = dataProvider;
        }

        public async Task<IActionResult> Index()
        {
            _newestDateTime = await _dataProvider.GetNewestDateTime();
            ViewBag.NewestDateTime = _newestDateTime.ToString("yyyy-MM-dd");
            return View();
        }

        public async Task<IActionResult> Villages()
        {
            ViewData["Server"] = _dataProvider.ServerUrl;
            _newestDateTime = await _dataProvider.GetNewestDateTime();

            var players = await _dataProvider.GetPlayers(_newestDateTime);
            return View(players);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}