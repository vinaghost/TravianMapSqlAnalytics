using MapSqlQuery.Services.Interfaces;
using MapSqlQuery.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MapSqlQuery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataProvide _dataProvider;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IDataProvide dataProvider, IConfiguration configuration)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.NewestDateTime = DateTime.Today.ToString("yyyy-MM-dd");
            ViewData["Server"] = _configuration["WorldUrl"];
            return View();
        }

        public async Task<IActionResult> InactivePlayers(InactivePlayerViewModel viewModel = null!)
        {
            ViewData["Server"] = _configuration["WorldUrl"];

            var players = await Task.Run(() => _dataProvider.GetInactivePlayerData(viewModel.FormInput));

            var days = players.FirstOrDefault()?.Population.Count ?? 0;
            viewModel.FormInput.Days = days;
            ViewData["Days"] = days;

            viewModel.Players = players;
            return View(viewModel);
        }

        public IActionResult VillageFilter(VillageFilterViewModel viewModel)
        {
            ViewData["Server"] = _configuration["WorldUrl"];

            viewModel.Villages = _dataProvider.GetVillageData(viewModel.FormInput);
            viewModel.Alliances = _dataProvider.GetAllianceSelectList();
            viewModel.Tribes = _dataProvider.GetTribeSelectList();
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}