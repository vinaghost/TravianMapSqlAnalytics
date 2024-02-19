using Core.Features.GetInactiveFarms;
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
    }
}