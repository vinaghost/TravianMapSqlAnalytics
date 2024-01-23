using Core.Features.GetVillageContainsDistance;
using Core.Features.GetVillageContainsPopulationHistory;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
	public class VillagesController : Controller
	{
		public IActionResult Index(VillageContainsDistanceParameters parameters, [FromServices] IValidator<VillageContainsDistanceParameters> validator)
		{
			var result = validator.Validate(parameters);
			result.AddToModelState(ModelState);

			return View(parameters);
		}

		public IActionResult PopulationHistory(VillageContainsPopulationHistoryParameters parameters, [FromServices] IValidator<VillageContainsPopulationHistoryParameters> validator)
		{
			var result = validator.Validate(parameters);
			result.AddToModelState(ModelState);

			return View(parameters);
		}
	}
}