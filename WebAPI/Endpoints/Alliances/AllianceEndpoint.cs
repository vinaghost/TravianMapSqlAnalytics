using FastEndpoints;
using Features.Alliances;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Requests;

namespace WebAPI.Endpoints.Alliances
{
    [HttpGet("/alliances/{Id}"), AllowAnonymous]
    public class AllianceEndpoint(IMediator mediator) :
        Endpoint<IdRequest,
                Results<Ok<AllianceDto>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<AllianceDto>,
            NotFound>>
            ExecuteAsync(IdRequest rq, CancellationToken ct)
        {
            var info = await _mediator.Send(new GetAllianceQuery(rq.Id), ct);
            if (info is null) return TypedResults.NotFound();
            return TypedResults.Ok(info);
        }
    }
}