using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Queries;
using X.PagedList;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(404)]
    public class VillagesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Village>), 200)]
        public async Task<IActionResult> Get([FromBody] VillageParameters villageParameters)
        {
            var villages = await _mediator.Send(new GetVillagesQuery(villageParameters));
            Response.Headers.Append("X-Pagination", villages.ToXpagination().ToJson());
            return Ok(villages);
        }

        [HttpGet("population_history")]
        [ProducesResponseType(typeof(IPagedList<VillageContainPopulationHistory>), 200)]
        public async Task<IActionResult> Get([FromBody] VillageContainsPopulationHistoryParameters villageParameters)
        {
            var villages = await _mediator.Send(new GetChangePopulationVillagesQuery(villageParameters));
            Response.Headers.Append("X-Pagination", villages.ToXpagination().ToJson());
            return Ok(villages);
        }
    }
}