using FastEndpoints;
using Features.Players;
using Features.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Groups;

namespace WebAPI.Endpoints.Players
{
    public record PlayerRequest(string ServerUrl, int Id) : IServerUrlRequest, IIdRequest;

    public class PlayerRequestValidator : Validator<PlayerRequest>
    {
        public PlayerRequestValidator()
        {
            Include(new ServerUrlRequestValidator());
        }
    }

    public class PlayerEndpoint(IMediator mediator) : Endpoint<PlayerRequest, Results<Ok<PlayerDto>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Get("{Id}");
            AllowAnonymous();
            Group<Player>();
        }

        public override async Task<Results<Ok<PlayerDto>, NotFound>> ExecuteAsync(PlayerRequest rq, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetPlayerByIdQuery(rq.Id), ct);
            if (result is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(result);
        }
    }
}