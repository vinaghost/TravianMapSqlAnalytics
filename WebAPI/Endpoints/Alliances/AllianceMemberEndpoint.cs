using FastEndpoints;
using Features.Alliances;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Requests;

namespace WebAPI.Endpoints.Alliances
{
    [HttpGet("/alliances/{Id}/members"), AllowAnonymous]
    public class AllianceMemberEndpoint(IMediator mediator) :
        Endpoint<IdRequest,
                Results<Ok<List<PlayerDto>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<List<PlayerDto>>,
            NotFound>>
            ExecuteAsync(IdRequest rq, CancellationToken ct)
        {
            var members = await _mediator.Send(new GetAllianceMemberQuery(rq.Id), ct);
            if (members.Count == 0) return TypedResults.NotFound();
            return TypedResults.Ok(members);
        }
    }
}