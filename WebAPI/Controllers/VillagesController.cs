using Core;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Output;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VillagesController(ServerDbContext dbContext) : ControllerBase
    {
        private readonly ServerDbContext _dbContext = dbContext;

        [HttpGet]
        public Village Get()
        {
            var village = _dbContext.Villages
                 .OrderByDescending(x => x.Population)
                 .Select(x => new Village(x.Name, x.Population))
                 .First();
            return village;
        }
    }
}