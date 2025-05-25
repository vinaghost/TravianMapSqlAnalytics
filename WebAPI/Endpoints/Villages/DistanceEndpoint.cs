using FastEndpoints;
using Features.Queries.Villages.ByDistance;
using Features.Queries.Villages.Shared;
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

    public class PlayersEndpoint : Endpoint<DistanceRequest, Results<Ok<PagedListResponse<VillageDto>>, NotFound>>
    {
        private readonly GetVillagesByParametersQuery.Handler _getVillagesByParameters;

        public PlayersEndpoint(GetVillagesByParametersQuery.Handler getVillagesByParameters)
        {
            _getVillagesByParameters = getVillagesByParameters;
        }

        public override void Configure()
        {
            Get("distance");
            AllowAnonymous();
            Group<Village>();
        }

        public override async Task<Results<Ok<PagedListResponse<VillageDto>>, NotFound>> ExecuteAsync(DistanceRequest rq, CancellationToken ct)
        {
            var result = await _getVillagesByParameters.HandleAsync(new(rq), ct);
            return TypedResults.Ok(new PagedListResponse<VillageDto>(result));
        }
    }
}