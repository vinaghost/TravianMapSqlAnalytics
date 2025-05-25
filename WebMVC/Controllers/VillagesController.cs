using Features.Queries.Populations;
using Features.Queries.Populations.Shared;
using Features.Queries.Villages;
using Features.Queries.Villages.ByDistance;
using Features.Queries.Villages.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Extensions;
using WebMVC.Models.ViewModel.Villages;

namespace WebMVC.Controllers
{
    public class VillagesController : Controller
    {
        [HttpGet]
        public IActionResult Inactives()
        {
            ViewBag.IsInput = false;
            return View(new InactiveViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Inactives(
            GetInactiveVillagesParameters parameters,
            [FromServices] GetInactiveVillagesQuery.Handler getInactiveVillagesQuery,
            [FromServices] GetVillagesPopulationHistoryByParametersQuery.Handler getVillagesPopulationHistoryByParametersQuery,
            [FromServices] IValidator<GetInactiveVillagesParameters> validator)
        {
            ViewBag.IsInput = true;

            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);

            if (!ViewData.ModelState.IsValid)
            {
                return View(new InactiveViewModel { Parameters = parameters });
            }

            var villages = await getInactiveVillagesQuery.HandleAsync(new(parameters));

            if (villages.Count <= 0)
            {
                return View(new InactiveViewModel { Parameters = parameters, Villages = villages });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = [.. villages.Select(p => p.VillageId)],
                Days = 7,
            };
            var population = await getVillagesPopulationHistoryByParametersQuery.HandleAsync(new(populationParameters));
            return View(new InactiveViewModel { Parameters = parameters, Villages = villages, Population = population });
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.IsInput = false;
            return View(new IndexViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            VillagesParameters parameters,
            [FromServices] GetVillagesByParametersQuery.Handler getVillagesByParametersQuery,
            [FromServices] GetVillagesPopulationHistoryByParametersQuery.Handler getVillagesPopulationHistoryByParametersQuery,
            [FromServices] IValidator<VillagesParameters> validator)
        {
            ViewBag.IsInput = true;

            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);

            if (!ViewData.ModelState.IsValid)
            {
                return View(new IndexViewModel { Parameters = parameters });
            }
            var villages = await getVillagesByParametersQuery.HandleAsync(new(parameters));

            if (villages.Count <= 0)
            {
                return View(new IndexViewModel { Parameters = parameters, Villages = villages });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = [.. villages.Select(p => p.VillageId)],
                Days = 14,
            };
            var population = await getVillagesPopulationHistoryByParametersQuery.HandleAsync(new(populationParameters));
            return View(new IndexViewModel { Parameters = parameters, Villages = villages, Population = population });
        }
    }
}