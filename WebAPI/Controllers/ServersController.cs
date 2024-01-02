using Core.Models;
using Core.Parameters;
using Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Server>), 200)]
        public async Task<IActionResult> Get([FromBody] ServerParameters serverParameters)
        {
            var servers = await _mediator.Send(new GetServerQuery(serverParameters));
            return Ok(servers);
        }
    }
}