using Core;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Output;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServersController(ServerListDbContext dbContext) : ControllerBase
    {
        private readonly ServerListDbContext _dbContext = dbContext;

        [HttpGet]
        public IEnumerable<Server> Get()
        {
            var servers = _dbContext.Servers
                 .Select(x => new Server(x.Url, x.Zone, x.StartDate, x.AllianceCount, x.PlayerCount, x.VillageCount));
            return servers;
        }
    }
}