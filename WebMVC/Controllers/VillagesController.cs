using Features.Populations;
using Features.Populations.Shared;
using Features.Villages;
using Features.Villages.ByDistance;
using Features.Villages.Shared;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models.ViewModel.Villages;

namespace WebMVC.Controllers
{
    public class VillagesController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public IActionResult Inactives()
        {
            ViewBag.IsInput = false;
            return View(new InactiveViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Inactives(GetInactiveVillagesParameters parameters, [FromServices] IValidator<GetInactiveVillagesParameters> validator)
        {
            ViewBag.IsInput = true;

            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);

            if (!ViewData.ModelState.IsValid)
            {
                return View(new InactiveViewModel { Parameters = parameters });
            }

            var villages = await _mediator.Send(new GetInactiveVillagesQuery(parameters));

            if (villages.Count <= 0)
            {
                return View(new InactiveViewModel { Parameters = parameters, Villages = villages });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = [.. villages.Select(p => p.VillageId)],
                Days = 7,
            };
            var population = await _mediator.Send(new GetVillagesPopulationHistoryByParametersQuery(populationParameters));
            return View(new InactiveViewModel { Parameters = parameters, Villages = villages, Population = population });
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.IsInput = false;
            return View(new IndexViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(VillagesParameters parameters, [FromServices] IValidator<VillagesParameters> validator)
        {
            ViewBag.IsInput = true;

            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);

            if (!ViewData.ModelState.IsValid)
            {
                return View(new IndexViewModel { Parameters = parameters });
            }

            var villages = await _mediator.Send(new GetVillagesByParameters(parameters));

            if (villages.Count <= 0)
            {
                return View(new IndexViewModel { Parameters = parameters, Villages = villages });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = [.. villages.Select(p => p.VillageId)],
                Days = 14,
            };
            var population = await _mediator.Send(new GetVillagesPopulationHistoryByParametersQuery(populationParameters));
            return View(new IndexViewModel { Parameters = parameters, Villages = villages, Population = population });
        }
    }
}