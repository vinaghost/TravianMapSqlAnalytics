using Features.Populations;
using Features.Populations.Shared;
using Features.Villages;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models.ViewModel.Villages;
using X.PagedList;

namespace WebMVC.Controllers
{
    public class VillagesController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        public IActionResult Inactives()
        {
            ViewBag.IsInput = false;
            return View();
        }

        public async Task<IActionResult> Inactives(GetInactiveParameters parameters, [FromServices] IValidator<GetInactiveParameters> validator)
        {
            ViewBag.IsInput = true;

            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);

            if (!ViewData.ModelState.IsValid)
            {
                return View(new InactiveViewModel { Parameters = parameters });
            }

            var villages = await _mediator.Send(new GetInactiveQuery(parameters));

            if (villages.Count <= 0)
            {
                return View(new InactiveViewModel { Parameters = parameters, Villages = villages });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = [.. villages.Select(p => p.PlayerId)],
                Days = 7,
            };
            var population = await _mediator.Send(new GetPlayersPopulationHistoryByIdQuery(populationParameters));
            return View(new InactiveViewModel { Parameters = parameters, Villages = villages, Population = population });
        }

        public IActionResult Index()
        {
            ViewBag.IsInput = false;
            return View();
        }

        public async Task<IActionResult> Index(GetVillagesParameters parameters, [FromServices] IValidator<GetVillagesParameters> validator)
        {
            ViewBag.IsInput = true;

            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);

            if (!ViewData.ModelState.IsValid)
            {
                return View(new IndexViewModel { Parameters = parameters });
            }

            var villages = await _mediator.Send(new GetVillagesQuery(parameters));

            if (villages.Count <= 0)
            {
                return View(new IndexViewModel { Parameters = parameters, Villages = villages });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = [.. villages.Select(p => p.PlayerId)],
                Days = 7,
            };
            var population = await _mediator.Send(new GetPlayersPopulationHistoryByIdQuery(populationParameters));
            return View(new IndexViewModel { Parameters = parameters, Villages = villages, Population = population });
        }
    }
}