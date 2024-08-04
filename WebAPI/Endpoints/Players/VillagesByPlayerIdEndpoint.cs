using FastEndpoints;
using Features.Shared.Parameters;
using Features.Villages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using X.PagedList;

namespace WebAPI.Endpoints.Villages
{
    public record VillagesByPlayerIdRequest(string ServerUrl, int Id, int PageNumber, int PageSize) : IServerUrlRequest, IIdRequest, IPaginationParameters;

    public class VillagesByAllianceIdRequestValidator : Validator<VillagesByPlayerIdRequest>
    {
        public VillagesByAllianceIdRequestValidator()
        {
            Include(new IdRequestValidator());
            Include(new PaginationParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    public class VillagesByAllianceIdResponse(IPagedList<VillageDto> pagedList) : PagedListResponse<VillageDto>(pagedList);

    [HttpGet("/players/{Id}/villages"), AllowAnonymous]
    public class VillagesByPlayerIdEndpoint(IMediator mediator) :
        Endpoint<VillagesByPlayerIdRequest,
                Results<Ok<VillagesByAllianceIdResponse>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<VillagesByAllianceIdResponse>, NotFound>>
            ExecuteAsync(VillagesByPlayerIdRequest rq, CancellationToken ct)
        {
            var parameters = new GetVillagesParameters
            {
                PageNumber = rq.PageNumber,
                PageSize = rq.PageSize,
                Players = [rq.Id]
            };

            var villages = await _mediator.Send(new GetVillagesQuery(parameters), ct);
            return TypedResults.Ok(new VillagesByAllianceIdResponse(villages));
        }
    }
}