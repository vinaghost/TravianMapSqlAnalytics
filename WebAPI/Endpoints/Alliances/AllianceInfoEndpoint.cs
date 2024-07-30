using FastEndpoints;
using Features.Alliances;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Requests;

namespace WebAPI.Endpoints.Alliances
{
    [HttpGet("/alliances/{AllianceId}/info"), AllowAnonymous]
    public class AllianceInfoEndpoint(IMediator mediator) :
        Endpoint<AllianceIdRequest,
                Results<Ok<AllianceDto>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<AllianceDto>,
            NotFound>>
            ExecuteAsync(AllianceIdRequest rq, CancellationToken ct)
        {
            var info = await _mediator.Send(new GetAllianceInfoQuery(rq.AllianceId), ct);
            if (info is null) return TypedResults.NotFound();
            return TypedResults.Ok(info);
        }
    }
}