using Features.GetInactiveFarms;
using Features.GetNeighbors;
using Features.Shared.Dtos;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMVC.ViewModels.Villages;
using X.PagedList;

namespace WebMVC.Controllers
{
    public class VillagesController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        public async Task<IActionResult> InactiveFarms(InactiveFarmParameters parameters, [FromServices] IValidator<InactiveFarmParameters> validator)
        {
            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);

            IPagedList<VillageDataDto>? data = null;

            if (ViewData.ModelState.IsValid)
            {
                data = await _mediator.Send(new InactiveFarmsQuery(parameters));
            }

            var viewModel = new InactiveFarmViewModel
            {
                Parameters = parameters,
                Data = data,
            };

            return View(viewModel);
        }

        public IActionResult Neighbors(NeighborsParameters parameters, [FromServices] IValidator<NeighborsParameters> validator)
        {
            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);
            return View(parameters);
        }
    }
}