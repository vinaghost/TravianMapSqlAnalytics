using FastEndpoints;
using Features.Queries.Players;
using Features.Shared.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using WebAPI.Groups;

namespace WebAPI.Endpoints.Players
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

    public class PlayersEndpoint : Endpoint<PlayersRequest, Results<Ok<PagedListResponse<PlayerDto>>, NotFound>>
    {
        private readonly GetPlayersByNameQuery.Handler _getPlayersByNameQuery;

        public PlayersEndpoint(GetPlayersByNameQuery.Handler getPlayersByNameQuery)
        {
            _getPlayersByNameQuery = getPlayersByNameQuery;
        }

        public override void Configure()
        {
            Get("");
            AllowAnonymous();
            Group<Player>();
        }

        public override async Task<Results<Ok<PagedListResponse<PlayerDto>>, NotFound>> ExecuteAsync(PlayersRequest rq, CancellationToken ct)
        {
            var result = await _getPlayersByNameQuery.HandleAsync(new(rq), ct);
            return TypedResults.Ok(new PagedListResponse<PlayerDto>(result));
        }
    }
}