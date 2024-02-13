using Core.Features.GetInactiveFarms;
using Core.Features.GetVillageContainsDistance;
using Core.Features.GetVillageContainsPopulationHistory;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace WebMVC.Controllers
{
    public class VillagesController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

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

        public async Task<IActionResult> InactiveFarms(InactiveFarmParameters parameters, [FromServices] IValidator<InactiveFarmParameters> validator)
        {
            var result = validator.Validate(parameters);
            if (!result.IsValid) return View(new PagedList<InactiveFarmDto>(null, 1, 1));
            var dto = await _mediator.Send(new GetInactiveFarmsQuery(parameters));
            return View(dto);
        }
    }
}