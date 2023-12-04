using Microsoft.AspNetCore.Mvc;

namespace MapSqlAspNetCoreMVC.ViewComponents
{
    public class ServerPicker : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var server = HttpContext.Request.Cookies["server"];
            List<string> availableServers = ["xyz", "acb", "asd"];
            var currentServer = string.IsNullOrWhiteSpace(server) ? availableServers[0] : server;
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