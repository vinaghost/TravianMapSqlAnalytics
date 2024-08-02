using FastEndpoints;
using Features.Villages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using X.PagedList;

namespace WebAPI.Endpoints.Villages
{
    public record AllVillageRequest(string ServerUrl) : GetVillagesParameters, IServerUrlRequest;

    public class AllVillageResponse(IPagedList<VillageDto> pagedList) : PagedListResponse<VillageDto>(pagedList);

    public class AllVillageRequestValidator : Validator<AllVillageRequest>
    {
        public AllVillageRequestValidator()
        {
            Include(new GetVillagesParametersValidator());
        }
    }

    [HttpGet("/villages/"), AllowAnonymous]
    public class AllVillageEndpoint(IMediator mediator) :
        Endpoint<AllVillageRequest,
                Results<Ok<AllVillageResponse>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<AllVillageResponse>,
            NotFound>>
            ExecuteAsync(AllVillageRequest rq, CancellationToken ct)
        {
            var data = await _mediator.Send(new GetVillagesQuery(rq), ct);
            return TypedResults.Ok(new AllVillageResponse(data));
        }
    }
}