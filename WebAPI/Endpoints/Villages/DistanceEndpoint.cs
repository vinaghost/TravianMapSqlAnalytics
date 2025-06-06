using FastEndpoints;
using Features.Villages;
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

    public class PlayersEndpoint : Endpoint<DistanceRequest, Results<Ok<PagedListResponse<DetailVillageDto>>, NotFound>>
    {
        private readonly GetVillagesQuery.Handler _getVillagesByParameters;

        public PlayersEndpoint(GetVillagesQuery.Handler getVillagesByParameters)
        {
            _getVillagesByParameters = getVillagesByParameters;
        }

        public override void Configure()
        {
            Get("distance");
            AllowAnonymous();
            Group<Village>();
        }

        public override async Task<Results<Ok<PagedListResponse<DetailVillageDto>>, NotFound>> ExecuteAsync(DistanceRequest rq, CancellationToken ct)
        {
            var result = await _getVillagesByParameters.HandleAsync(new(rq), ct);
            return TypedResults.Ok(new PagedListResponse<DetailVillageDto>(result));
        }
    }
}