using MapSqlAspNetCoreMVC.CQRS.Queries;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.View;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MapSqlAspNetCoreMVC.Controllers
{
    public class PlayerWithDetailController : Controller
    {
        private readonly ILogger<PlayerWithDetailController> _logger;

        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public PlayerWithDetailController(ILogger<PlayerWithDetailController> logger, IConfiguration configuration, IMediator mediator)
        {
            _logger = logger;

            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(PlayerWithDetailInput input)
        {
            input ??= new PlayerWithDetailInput();
            var viewModel = await GetViewModel(input);
            return View(viewModel);
        }

        private async Task<PlayerWithDetailViewModel> GetViewModel(PlayerWithDetailInput input)
        {
            if (!string.IsNullOrWhiteSpace(input.PlayerName))
            {
                var player = await _mediator.Send(new GetPlayerWithDetailByInputQuery(input));
                var dates = await _mediator.Send(new GetDateBeforeQuery(input.Days));

                var viewModel = new PlayerWithDetailViewModel
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
                var viewModel = new PlayerWithDetailViewModel
                {
                    Server = _configuration["WorldUrl"],
                    Input = input,
                };
                return viewModel;
            }
        }
    }
}