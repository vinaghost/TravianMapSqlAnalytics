﻿using Core.Features.GetVillageContainsDistance;
using Core.Features.GetVillageContainsPopulationHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Extensions;
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
        [ProducesResponseType(typeof(IPagedList<VillageContainsDistanceDto>), 200)]
        public async Task<IActionResult> Get([FromBody] VillageContainsDistanceParameters villageParameters)
        {
            var villages = await _mediator.Send(new GetVillageContainsDistanceQuery(villageParameters));
            Response.Headers.Append("X-Pagination", villages.ToJson());
            return Ok(villages);
        }

        [HttpGet("population_history")]
        [ProducesResponseType(typeof(IPagedList<VillageContainPopulationHistory>), 200)]
        public async Task<IActionResult> Get([FromBody] VillageContainsPopulationHistoryParameters villageParameters)
        {
            var villages = await _mediator.Send(new GetVillageContainsPopulationHistoryQuery(villageParameters));
            Response.Headers.Append("X-Pagination", villages.ToJson());
            return Ok(villages);
        }
    }
}