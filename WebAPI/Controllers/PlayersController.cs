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
    public class PlayersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Player>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerParameters playerParameters)
        {
            var playerQuery = await _mediator.Send(new FilteredPlayerQuery(playerParameters.Alliances));

            var countPlayer = await _mediator.Send(new GetPlayerCountQuery());

            var rawPlayers = await playerQuery
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    PlayerName = x.Name,
                    VillageCount = x.Villages.Count(),
                    Population = x.Villages.Sum(x => x.Population),
                })
                .OrderByDescending(x => x.VillageCount)
                .ToPagedListAsync(playerParameters.PageNumber, playerParameters.PageSize, countPlayer);
            var alliances = await _mediator.Send(new AllianceInfoQuery(rawPlayers.Select(x => x.AllianceId)));

            var players = rawPlayers
                .Select(x =>
                {
                    var alliance = alliances[x.AllianceId];
                    return new Player(
                        x.AllianceId,
                        alliance,
                        x.PlayerId,
                        x.PlayerName,
                        x.VillageCount,
                        x.Population);
                });

            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());

            return Ok(players);
        }

        [HttpGet("change_population")]
        [ProducesResponseType(typeof(IPagedList<Player>), 200)]
        public async Task<IActionResult> Get([FromBody] ChangePopulationPlayerParameters playerParameters)
        {
            var playerQuery = await _mediator.Send(new FilteredPlayerQuery(playerParameters.Alliances));
            var date = playerParameters.Date.ToDateTime(TimeOnly.MinValue);
            var latestDate = await _mediator.Send(new GetLatestDateQuery());

            var countPlayer = await _mediator.Send(new GetPlayerCountQuery());

            var rawPlayers = await playerQuery
                .Select(x => new
                {
                    x.PlayerId,
                    Populations = x.Villages
                        .SelectMany(x => x.Populations
                                        .Where(x => x.Date == date || x.Date == latestDate))
                        .GroupBy(x => x.Date)
                        .OrderByDescending(x => x.Key)
                        .Select(x => x
                                    .OrderByDescending(x => x.Date)
                                    .Select(x => x.Population)
                                    .Sum())
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.PlayerId,
                    ChangePopulation = x.Populations.FirstOrDefault() - x.Populations.LastOrDefault(),
                })
                .OrderByDescending(x => x.ChangePopulation)
                .ToPagedListAsync(playerParameters.PageNumber, playerParameters.PageSize, countPlayer);

            //.Select(x => new
            //{
            //    x.AllianceId,
            //    x.PlayerId,
            //    x.PlayerName,
            //    ChangePopulation = x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
            //    Populations = x.Populations.Select(x => new Population(x.Population, x.Date))
            //})
            var players = rawPlayers;

            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());

            return Ok(players);
        }
    }
}