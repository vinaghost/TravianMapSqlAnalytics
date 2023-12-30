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

            var countPlayer = await _mediator.Send(new GetPlayerCountQuery(playerParameters.Alliances));

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

            var countPlayer = await _mediator.Send(new GetPlayerCountQuery(playerParameters.Alliances));

            var rawPlayers = await playerQuery
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    PlayerName = x.Name,
                    Populations = x.Villages
                        .SelectMany(x => x.Populations
                                        .Where(x => x.Date >= date))
                        .GroupBy(x => x.Date)
                        .OrderByDescending(x => x.Key)
                        .Select(x => new
                        {
                            Date = x.Key,
                            Population = x
                                    .OrderBy(x => x.Date)
                                    .Select(x => x.Population)
                                    .Sum(),
                        })
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    x.PlayerName,
                    ChangePopulation = x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                    Populations = x.Populations.Select(x => new PopulationRecord(x.Population, x.Date))
                })
                .OrderByDescending(x => x.ChangePopulation)
                .ToPagedListAsync(playerParameters.PageNumber, playerParameters.PageSize, countPlayer);

            var alliances = await _mediator.Send(new AllianceInfoQuery(rawPlayers.Select(x => x.AllianceId)));

            var players = rawPlayers
                .Select(x =>
                {
                    var alliance = alliances[x.AllianceId];
                    return new ChangePopulationPlayer(
                        x.AllianceId,
                        alliance,
                        x.PlayerId,
                        x.PlayerName,
                        x.ChangePopulation,
                        x.Populations);
                });

            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());

            return Ok(players);
        }

        [HttpGet("change_alliance")]
        [ProducesResponseType(typeof(IPagedList<Player>), 200)]
        public async Task<IActionResult> Get([FromBody] ChangeAlliancePlayerParameters playerParameters)
        {
            var playerQuery = await _mediator.Send(new FilteredPlayerQuery(playerParameters.Alliances));
            var date = playerParameters.Date.ToDateTime(TimeOnly.MinValue);

            var countPlayer = await _mediator.Send(new GetPlayerCountQuery(playerParameters.Alliances));

            var rawPlayers = await playerQuery
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    PlayerName = x.Name,
                    Alliances = x.Alliances
                        .Where(x => x.Date >= date)
                        .Select(x => new
                        {
                            x.Date,
                            x.AllianceId,
                        })
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    x.PlayerName,
                    ChangeAlliances = x.Alliances.DistinctBy(x => x.AllianceId).Count(),
                    x.Alliances
                })
                .OrderByDescending(x => x.ChangeAlliances)
                .ToPagedListAsync(playerParameters.PageNumber, playerParameters.PageSize, countPlayer);

            var oldAllianceId = rawPlayers.SelectMany(x => x.Alliances).DistinctBy(x => x.AllianceId).Select(x => x.AllianceId);
            var currentAllianceId = rawPlayers.Select(x => x.AllianceId);

            var alliances = await _mediator.Send(new AllianceInfoQuery(currentAllianceId.Concat(oldAllianceId)));

            var players = rawPlayers
                .Select(x => new ChangeAlliancePlayer(
                        x.AllianceId,
                        alliances[x.AllianceId],
                        x.PlayerId,
                        x.PlayerName,
                        x.ChangeAlliances,
                        x.Alliances.Select(alliance => new AllianceRecord(
                            alliance.AllianceId,
                            alliances[alliance.AllianceId],
                            alliance.Date))));

            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());

            return Ok(players);
        }
    }
}