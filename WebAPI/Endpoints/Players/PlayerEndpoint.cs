using FastEndpoints;
using Features.Players;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Requests;

namespace WebAPI.Endpoints.Players
{
    [HttpGet("/players/{Id}"), AllowAnonymous]
    public class PlayerEndpoint(IMediator mediator) :
        Endpoint<IdRequest,
                Results<Ok<PlayerDto>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<PlayerDto>,
            NotFound>>
            ExecuteAsync(IdRequest rq, CancellationToken ct)
        {
            var info = await _mediator.Send(new GetPlayerQuery(rq.Id), ct);
            return TypedResults.Ok(info);
        }
    }
}