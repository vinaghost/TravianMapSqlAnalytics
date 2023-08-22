using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.View;
using MapSqlAspNetCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace MapSqlAspNetCoreMVC.Controllers
{
    public class InactivePlayersController : Controller
    {
        private readonly ILogger<InactivePlayersController> _logger;
        private readonly IDataProvide _dataProvider;
        private readonly IConfiguration _configuration;

        public InactivePlayersController(ILogger<InactivePlayersController> logger, IDataProvide dataProvider, IConfiguration configuration)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(InactiveFormInput input)
        {
            input ??= new InactiveFormInput();
            var viewModel = await GetViewModel(input);
            return View(viewModel);
        }

        private async Task<InactivePlayerViewModel> GetViewModel(InactiveFormInput input)
        {
            var players = await _dataProvider.GetInactivePlayerData(input);
            var pagePlayers = players.ToPagedList(input.PageNumber, input.PageSize);
            var dates = _dataProvider.GetDateBefore(input.Days);

            var viewModel = new InactivePlayerViewModel
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