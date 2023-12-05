using MapSqlAspNetCoreMVC.CQRS.Queries;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.View;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace MapSqlAspNetCoreMVC.Controllers
{
    public class PlayerAllianceChangeController : Controller
    {
        private readonly ILogger<PlayerAllianceChangeController> _logger;

        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public PlayerAllianceChangeController(ILogger<PlayerAllianceChangeController> logger, IConfiguration configuration, IMediator mediator)
        {
            _logger = logger;

            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(PlayerWithAllianceInput input)
        {
            input ??= new PlayerWithAllianceInput();
            var viewModel = await GetViewModel(input);
            return View(viewModel);
        }

        private async Task<PlayerAllianceChangeViewModel> GetViewModel(PlayerWithAllianceInput input)
        {
            var players = await _mediator.Send(new GetPlayerWithAllianceByInputQuery(input));
            var pagePlayers = players.ToPagedList(input.PageNumber, input.PageSize);
            var dates = await _mediator.Send(new GetDateBeforeQuery(input.Days));

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