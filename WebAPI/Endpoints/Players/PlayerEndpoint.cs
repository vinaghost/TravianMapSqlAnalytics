using FastEndpoints;
using Features.Players;
using Features.Players.GetPlayerById;
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

    public class PlayerEndpoint : Endpoint<PlayerRequest, Results<Ok<PlayerDto>, NotFound>>
    {
        private readonly GetPlayerByIdQuery.Handler _getPlayerByIdQuery;

        public PlayerEndpoint(GetPlayerByIdQuery.Handler getPlayerByIdQuery)
        {
            _getPlayerByIdQuery = getPlayerByIdQuery;
        }

        public override void Configure()
        {
            Get("{Id}");
            AllowAnonymous();
            Group<Player>();
        }

        public override async Task<Results<Ok<PlayerDto>, NotFound>> ExecuteAsync(PlayerRequest rq, CancellationToken ct)
        {
            var result = await _getPlayerByIdQuery.HandleAsync(new(rq.Id), ct);
            if (result is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(result);
        }
    }
}