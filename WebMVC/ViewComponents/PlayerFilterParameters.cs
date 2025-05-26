using Features.Parameters;
using Features.Queries.Alliances;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.ViewComponents
{
    public class PlayerFilterParameters : ParametersViewComponent<IPlayerFilterParameters>
    {
        private readonly GetAlliancesByIdQuery.Handler _getAlliancesByIdQuery;

        public PlayerFilterParameters(GetAlliancesByIdQuery.Handler getAlliancesByIdQuery)
        {
            _getAlliancesByIdQuery = getAlliancesByIdQuery;
        }

        public override async Task<IViewComponentResult> InvokeAsync(IPlayerFilterParameters parameter)
        {
            ViewData["Alliances"] = await _getAlliancesByIdQuery.HandleAsync(new(new GetAlliancesByIdParameters(parameter.Alliances ?? [])));
            ViewData["ExcludeAlliances"] = await _getAlliancesByIdQuery.HandleAsync(new(new GetAlliancesByIdParameters(parameter.ExcludeAlliances ?? [])));
            return View(parameter);
        }
    }
}