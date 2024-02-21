using Core.Features.GetInactiveFarms;
using Core.Features.GetNeighbors;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class VillagesController : Controller
    {
        public IActionResult InactiveFarms(InactiveFarmParameters parameters, [FromServices] IValidator<InactiveFarmParameters> validator)
        {
            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);
            return View(parameters);
        }

        public IActionResult Neighbors(NeighborsParameters parameters, [FromServices] IValidator<NeighborsParameters> validator)
        {
            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);
            return View(parameters);
        }
    }
}