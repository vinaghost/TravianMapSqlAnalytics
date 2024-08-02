using FastEndpoints;
using Features.Players;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using X.PagedList;

namespace WebAPI.Endpoints.Players
{
    public record AllPlayerRequest(string ServerUrl) : GetPlayersParameters, IServerUrlRequest;

    [HttpGet("/players/"), AllowAnonymous]
    public class AllPlayerEnpoint(IMediator mediator) :
       Endpoint<AllPlayerRequest,
               Results<Ok<IPagedList<PlayerDto>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<IPagedList<PlayerDto>>,
            NotFound>>
            ExecuteAsync(AllPlayerRequest rq, CancellationToken ct)
        {
            var players = await _mediator.Send(new GetPlayersQuery(rq), ct);
            return TypedResults.Ok(players);
        }
    }
}