using FastEndpoints;
using Features.Populations;
using Features.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;

namespace WebAPI.Endpoints.Populations
{
    [HttpGet("/populations/players"), AllowAnonymous]
    public class PlayersPopulationHistoryByIdEndpoint(IMediator mediator) :
        Endpoint<PopulationRequest,
                Results<Ok<Dictionary<int, List<PopulationDto>>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<Dictionary<int, List<PopulationDto>>>, NotFound>>
            ExecuteAsync(PopulationRequest rq, CancellationToken ct)
        {
            var population = await _mediator.Send(new GetPlayersPopulationHistoryByIdQuery(rq), ct);

            return TypedResults.Ok(population);
        }
    }
}