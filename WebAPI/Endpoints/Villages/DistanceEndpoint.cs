using FastEndpoints;
using Features.Queries.Villages.Shared;
using Features.Villages.ByDistance;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using WebAPI.Groups;

namespace WebAPI.Endpoints.Villages
{
    public record DistanceRequest(string ServerUrl) : VillagesParameters, IServerUrlRequest;

    public class DistanceRequestValidator : Validator<DistanceRequest>
    {
        public DistanceRequestValidator()
        {
            Include(new VillagesParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    public class PlayersEndpoint(IMediator mediator) : Endpoint<DistanceRequest, Results<Ok<PagedListResponse<VillageDto>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Get("distance");
            AllowAnonymous();
            Group<Village>();
        }

        public override async Task<Results<Ok<PagedListResponse<VillageDto>>, NotFound>> ExecuteAsync(DistanceRequest rq, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetVillagesByParameters(rq), ct);
            return TypedResults.Ok(new PagedListResponse<VillageDto>(result));
        }
    }
}