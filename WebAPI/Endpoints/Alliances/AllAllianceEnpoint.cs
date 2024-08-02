using FastEndpoints;
using Features.Alliances;
using Features.Shared.Parameters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using X.PagedList;

namespace WebAPI.Endpoints.Alliances
{
    public record AllAllianceRequest(string ServerUrl) : NameFilterParameters, IServerUrlRequest;

    [HttpGet("/alliances/"), AllowAnonymous]
    public class AllAllianceEnpoint(IMediator mediator) :
       Endpoint<AllAllianceRequest,
               Results<Ok<IPagedList<AllianceDto>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<IPagedList<AllianceDto>>,
            NotFound>>
            ExecuteAsync(AllAllianceRequest rq, CancellationToken ct)
        {
            var alliances = await _mediator.Send(new GetAllAllianceQuery(rq), ct);
            return TypedResults.Ok(alliances);
        }
    }
}