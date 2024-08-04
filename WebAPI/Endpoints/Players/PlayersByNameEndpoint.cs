using FastEndpoints;
using Features.Players;
using Features.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using X.PagedList;

namespace WebAPI.Endpoints.Players
{
    public record PlayersByNameRequest(string ServerUrl) : GetPlayersByNameParameters, IServerUrlRequest;

    public class PlayersByNameRequestValidator : Validator<PlayersByNameRequest>
    {
        public PlayersByNameRequestValidator()
        {
            Include(new GetPlayersByNameParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    public class PlayersByNameResponse(IPagedList<PlayerDto> pagedList) : PagedListResponse<PlayerDto>(pagedList);

    [HttpGet("/players/{SearchTerm}"), AllowAnonymous]
    public class PlayersByNameEnpoint(IMediator mediator) :
       Endpoint<PlayersByNameRequest,
               Results<Ok<PlayersByNameResponse>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<PlayersByNameResponse>, NotFound>>
            ExecuteAsync(PlayersByNameRequest rq, CancellationToken ct)
        {
            var players = await _mediator.Send(new GetPlayersByNameQuery(rq), ct);
            return TypedResults.Ok(new PlayersByNameResponse(players));
        }
    }
}