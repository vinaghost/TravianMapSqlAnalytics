using MapSqlAspNetCoreMVC.CQRS.Queries;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.View;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MapSqlAspNetCoreMVC.Controllers
{
    public class InactivePlayersController : Controller
    {
        private readonly ILogger<InactivePlayersController> _logger;

        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public InactivePlayersController(ILogger<InactivePlayersController> logger, IConfiguration configuration, IMediator mediator)
        {
            _logger = logger;

            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(PlayerWithPopulationInput input)
        {
            input ??= new PlayerWithPopulationInput();
            var viewModel = await GetViewModel(input);
            return View(viewModel);
        }

        private async Task<InactivePlayersViewModel> GetViewModel(PlayerWithPopulationInput input)
        {
            var players = await _mediator.Send(new GetPlayerWithPopulationByInputQuery(input));
            var dates = await _mediator.Send(new GetDateBeforeQuery(input.Days));

            var viewModel = new InactivePlayersViewModel
            {
                Server = _configuration["WorldUrl"],
                Input = input,
                PlayerTotal = players.TotalItemCount,
                Dates = dates,
                Players = players
            };
            return viewModel;
        }
    }
}