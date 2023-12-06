using MapSqlAspNetCoreMVC.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MapSqlAspNetCoreMVC.ViewComponents
{
    public class ServerPicker : ViewComponent
    {
        private readonly IMediator _mediator;

        public ServerPicker(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentServer = HttpContext.Items["server"] as string;
            var availableServers = await _mediator.Send(new GetServerUrlsQuery());
            var model = new ServerPickerModel()
            {
                CurrentServer = currentServer,
                AvailableServers = availableServers,
            };

            return View(model);
        }
    }

    public class ServerPickerModel
    {
        public string CurrentServer { get; set; }
        public List<string> AvailableServers { get; set; }
    }
}