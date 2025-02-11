using FastEndpoints;
using Features.Players;
using Features.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Groups;
using X.PagedList;

namespace WebAPI.Endpoints
{
    public record PlayersRequest(string ServerUrl) : GetPlayersByNameParameters, IServerUrlRequest;

    public class PlayersRequestValidator : Validator<PlayersRequest>
    {
        public PlayersRequestValidator()
        {
            Include(new GetPlayersByNameParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    public class PlayersEndpoint(IMediator mediator) : Endpoint<PlayersRequest, Results<Ok<IPagedList<PlayerDto>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Get("");
            AllowAnonymous();
            Group<Player>();
        }

        public override async Task<Results<Ok<IPagedList<PlayerDto>>, NotFound>> ExecuteAsync(PlayersRequest rq, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetPlayersByNameQuery(rq), ct);
            return TypedResults.Ok(result);
        }
    }
}