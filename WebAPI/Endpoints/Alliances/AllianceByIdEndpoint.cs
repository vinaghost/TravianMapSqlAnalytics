using FastEndpoints;
using Features.Alliances;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;

namespace WebAPI.Endpoints.Alliances
{
    public record AllianceByIdRequest(string ServerUrl, int Id) : IServerUrlRequest, IIdRequest;

    public class AllianceByIdRequestValidator : Validator<AllianceByIdRequest>
    {
        public AllianceByIdRequestValidator()
        {
            Include(new IdRequestValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    [HttpGet("/alliances/{Id}"), AllowAnonymous]
    public class AllianceByIdEndpoint(IMediator mediator) :
        Endpoint<AllianceByIdRequest,
                Results<Ok<AllianceDto>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<AllianceDto>, NotFound>>
            ExecuteAsync(AllianceByIdRequest rq, CancellationToken ct)
        {
            var alliance = await _mediator.Send(new GetAllianceByIdQuery(rq.Id), ct);
            if (alliance is null) return TypedResults.NotFound();
            return TypedResults.Ok(alliance);
        }
    }
}