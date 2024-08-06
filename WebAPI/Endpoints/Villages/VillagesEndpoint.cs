using FastEndpoints;
using Features.Shared.Dtos;
using Features.Villages;
using Features.Villages.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using X.PagedList;

namespace WebAPI.Endpoints.Villages
{
    public record VillagesRequest(string ServerUrl) : GetVillagesParameters, IServerUrlRequest;

    public class VillagesRequestValidator : Validator<VillagesRequest>
    {
        public VillagesRequestValidator()
        {
            Include(new GetVillagesParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    public class VillagesResponse(IPagedList<VillageDto> pagedList) : PagedListResponse<VillageDto>(pagedList);

    [HttpGet("/villages/"), AllowAnonymous]
    public class VillagesEndpoint(IMediator mediator) :
        Endpoint<VillagesRequest,
                Results<Ok<VillagesResponse>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<VillagesResponse>, NotFound>>
            ExecuteAsync(VillagesRequest rq, CancellationToken ct)
        {
            var villages = await _mediator.Send(new GetVillagesQuery(rq), ct);
            return TypedResults.Ok(new VillagesResponse(villages));
        }
    }
}