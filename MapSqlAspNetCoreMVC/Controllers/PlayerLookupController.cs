using MapSqlQuery.Models.Input;
using MapSqlQuery.Models.View;
using MapSqlQuery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MapSqlQuery.Controllers
{
    public class PlayerLookupController : Controller
    {
        private readonly ILogger<PlayerLookupController> _logger;
        private readonly IDataProvide _dataProvider;
        private readonly IConfiguration _configuration;

        public PlayerLookupController(ILogger<PlayerLookupController> logger, IDataProvide dataProvider, IConfiguration configuration)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _configuration = configuration;
        }

        public IActionResult Index(PlayerLookupInput input)
        {
            input ??= new PlayerLookupInput();
            var viewModel = GetViewModel(input);
            return View(viewModel);
        }

        private PlayerLookupViewModel GetViewModel(PlayerLookupInput input)
        {
            if (!string.IsNullOrWhiteSpace(input.PlayerName))
            {
                var player = _dataProvider.GetPlayerInfo(input);
                var dates = _dataProvider.GetDateBefore(input.Days);

                var viewModel = new PlayerLookupViewModel
                {
                    Server = _configuration["WorldUrl"],
                    Input = input,
                    Player = player,
                    Dates = dates,
                };
                return viewModel;
            }
            else
            {
                var viewModel = new PlayerLookupViewModel
                {
                    Server = _configuration["WorldUrl"],
                    Input = input,
                };
                return viewModel;
            }



        }
    }
}