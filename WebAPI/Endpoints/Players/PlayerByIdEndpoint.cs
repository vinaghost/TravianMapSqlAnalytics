using FastEndpoints;
using Features.Players;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;

namespace WebAPI.Endpoints.Players
{
    public record PlayerByIdRequest(string ServerUrl, int Id) : IServerUrlRequest, IIdRequest;

    public class PlayerByIdRequestValidator : Validator<PlayerByIdRequest>
    {
        public PlayerByIdRequestValidator()
        {
            Include(new IdRequestValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    [HttpGet("/players/{Id}"), AllowAnonymous]
    public class PlayerByIdEndpoint(IMediator mediator) :
        Endpoint<PlayerByIdRequest,
                Results<Ok<PlayerDto>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<PlayerDto>, NotFound>>
            ExecuteAsync(PlayerByIdRequest rq, CancellationToken ct)
        {
            var player = await _mediator.Send(new GetPlayerByIdQuery(rq.Id), ct);
            return TypedResults.Ok(player);
        }
    }
}