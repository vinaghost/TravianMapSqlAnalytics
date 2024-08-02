using FastEndpoints;
using Features.Players;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;

namespace WebAPI.Endpoints.Alliances
{
    [HttpGet("/alliances/{Id}/members"), AllowAnonymous]
    public class MemberEndpoint(IMediator mediator) :
        Endpoint<IdRequest,
                Results<Ok<List<PlayerDto>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<List<PlayerDto>>,
            NotFound>>
            ExecuteAsync(IdRequest rq, CancellationToken ct)
        {
            var parameters = new GetPlayersParameters
            {
                PageNumber = 1,
                PageSize = 120,
                Alliances = [rq.Id]
            };

            var members = await _mediator.Send(new GetPlayersQuery(parameters), ct);
            return TypedResults.Ok(members.ToList());
        }
    }
}