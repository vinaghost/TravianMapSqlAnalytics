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
            ViewBag.NewestDateTime = _dataProvider.NewestDateStr;
            ViewData["Server"] = _configuration["WorldUrl"];
            return View();
        }

        public async Task<IActionResult> InactivePlayers(InactivePlayerViewModel viewModel = null!)
        {
            ViewData["Server"] = _configuration["WorldUrl"];
            var days = viewModel.FormInput?.Days ?? 3;
            ViewData["Days"] = days;

            var players = await _dataProvider.GetInactivePlayerData(_dataProvider.NewestDate, days);
            viewModel.Players = players;
            return View(viewModel);
        }

        public async Task<IActionResult> VillageFilter(VillageFilterViewModel viewModel)
        {
            ViewData["Server"] = _configuration["WorldUrl"];

            viewModel.Villages = await _dataProvider.GetVillageData(viewModel.FormInput);
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