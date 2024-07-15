using Features.SearchAlliance;
using Features.Shared.Parameters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.ViewComponents
{
    public class PlayerFilterParameters(IMediator mediator) : ParametersViewComponent<IPlayerFilterParameters>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<IViewComponentResult> InvokeAsync(IPlayerFilterParameters parameter)
        {
            ViewData["Alliances"] = await _mediator.Send(new SearchAllianceByIdQuery(parameter.Alliances));
            ViewData["ExcludeAlliances"] = await _mediator.Send(new SearchAllianceByIdQuery(parameter.ExcludeAlliances));
            return View(parameter);
        }
    }
}