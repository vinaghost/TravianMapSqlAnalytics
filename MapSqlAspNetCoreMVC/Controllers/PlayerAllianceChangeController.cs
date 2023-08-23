using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.View;
using MapSqlAspNetCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace MapSqlAspNetCoreMVC.Controllers
{
    public class PlayerAllianceChangeController : Controller
    {
        private readonly ILogger<PlayerAllianceChangeController> _logger;
        private readonly IDataProvide _dataProvider;
        private readonly IConfiguration _configuration;

        public PlayerAllianceChangeController(ILogger<PlayerAllianceChangeController> logger, IDataProvide dataProvider, IConfiguration configuration)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(PlayerWithAllianceInput input)
        {
            input ??= new PlayerWithAllianceInput();
            var viewModel = await GetViewModel(input);
            return View(viewModel);
        }

        private async Task<PlayerAllianceChangeViewModel> GetViewModel(PlayerWithAllianceInput input)
        {
            var players = await _dataProvider.GetPlayerChangeAlliance(input);
            var pagePlayers = players.ToPagedList(input.PageNumber, input.PageSize);
            var dates = _dataProvider.GetDateBefore(input.Days);

            var viewModel = new PlayerAllianceChangeViewModel
            {
                Server = _configuration["WorldUrl"],
                Input = input,
                PlayerTotal = players.Count,
                Dates = dates,
                Players = pagePlayers
            };
            return viewModel;
        }
    }
}