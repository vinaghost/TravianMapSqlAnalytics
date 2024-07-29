using FastEndpoints;
using Features.GetAllianceData;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebAPI.Endpoints
{
    public record AlliancesDto(int AllianceId);

    public class AlliancesEndpoint(IMediator mediator) : Endpoint<AlliancesDto,
                                        Results<Ok<AllianceDataDto>,
                                           NotFound,
                                           ProblemDetails>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Get("/alliances");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<AllianceDataDto>,
                                           NotFound,
                                           ProblemDetails>>
            ExecuteAsync(AlliancesDto rq, CancellationToken ct)
        {
            var alliance = await _mediator.Send(new GetAllianceDataQuery(rq.AllianceId), ct);

            if (alliance is null) return TypedResults.NotFound();

            return TypedResults.Ok(alliance);
        }
    }
}