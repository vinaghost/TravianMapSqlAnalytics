using FastEndpoints;
using Features.Alliances;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebAPI.Endpoints
{
    public record AlliancesDto(int AllianceId);
    public record AllianceDataDto(AllianceDto Alliance, IList<PlayerDto> Players);

    [HttpGet("/alliances/{AllianceId}"), AllowAnonymous]
    public class AlliancesEndpoint(IMediator mediator) : Endpoint<AlliancesDto,
                                        Results<Ok<AllianceDataDto>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<Ok<AllianceDataDto>,
                                           NotFound>>
            ExecuteAsync(AlliancesDto rq, CancellationToken ct)
        {
            var info = await _mediator.Send(new GetAllianceInfoQuery(rq.AllianceId), ct);
            if (info is null) return TypedResults.NotFound();
            var members = await _mediator.Send(new GetAllianceMemberQuery(rq.AllianceId), ct);

            var data = new AllianceDataDto(info, members);
            return TypedResults.Ok(data);
        }
    }
}