using MapSqlAspNetCoreMVC.CQRS.Queries;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.View;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MapSqlAspNetCoreMVC.Controllers
{
    public class VillageController : Controller
    {
        private readonly ILogger<VillageController> _logger;

        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public VillageController(ILogger<VillageController> logger, IConfiguration configuration, IMediator mediator)
        {
            _logger = logger;
            _configuration = configuration;

            _mediator = mediator;
        }

        public async Task<IActionResult> Index(VillageInput input)
        {
            input ??= new VillageInput();
            var viewModel = await GetViewModel(input);
            return View(viewModel);
        }

        private async Task<VillageViewModel> GetViewModel(VillageInput input)
        {
            var villages = await _mediator.Send(new GetVillageByInputQuery(input));
            var alliances = await _mediator.Send(new GetAllianceSelectListQuery());
            var tribes = await _mediator.Send(new GetTribeSelectListQuery());

            var viewModel = new VillageViewModel
            {
                Server = _configuration["WorldUrl"],
                Input = input,
                VillageTotal = villages.TotalItemCount,
                Villages = villages,
                Alliances = alliances,
                Tribes = tribes
            };
            return viewModel;
        }
    }
}