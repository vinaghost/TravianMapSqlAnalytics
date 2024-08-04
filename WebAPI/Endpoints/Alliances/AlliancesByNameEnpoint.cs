using FastEndpoints;
using Features.Alliances;
using Features.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using X.PagedList;

namespace WebAPI.Endpoints.Alliances
{
    public record AlliancesByNameRequest(string ServerUrl) : GetAlliancesByNameParameters, IServerUrlRequest;

    public class AlliancesByNameRequestValidator : Validator<AlliancesByNameRequest>
    {
        public AlliancesByNameRequestValidator()
        {
            Include(new GetAllianceByNameParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    public class AlliancesByNameResponse(IPagedList<AllianceDto> pagedList) : PagedListResponse<AllianceDto>(pagedList);

    [HttpGet("/alliances/{SearchTerm}"), AllowAnonymous]
    public class AlliancesByNameEnpoint(IMediator mediator) :
       Endpoint<AlliancesByNameRequest,
               Results<Ok<AlliancesByNameResponse>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<AlliancesByNameResponse>,
            NotFound>>
            ExecuteAsync(AlliancesByNameRequest rq, CancellationToken ct)
        {
            var alliances = await _mediator.Send(new GetAlliancesByNameQuery(rq), ct);
            return TypedResults.Ok(new AlliancesByNameResponse(alliances));
        }
    }
}