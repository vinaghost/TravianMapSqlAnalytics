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

        public async Task<IActionResult> Inactives(InactiveParameters parameters, [FromServices] IValidator<InactiveParameters> validator)
        {
            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);

            IPagedList<VillageDataDto>? data = null;

            if (ViewData.ModelState.IsValid && parameters.IsUserInput)
            {
                data = await _mediator.Send(new GetInactiveQuery(parameters));
            }

            var viewModel = new InactiveViewModel
            {
                Parameters = parameters,
                Data = data,
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Neighbors(NeighborsParameters parameters, [FromServices] IValidator<NeighborsParameters> validator)
        {
            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);

            IPagedList<VillageDataDto>? data = null;

            if (ViewData.ModelState.IsValid && parameters.IsUserInput)
            {
                data = await _mediator.Send(new GetNeighborsQuery(parameters));
            }

            var viewModel = new NeighborViewModel
            {
                Parameters = parameters,
                Data = data,
            };

            return View(viewModel);
        }
    }
}