using FastEndpoints;
using Features.Dtos;
using Features.Queries.Populations;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Groups;

namespace WebAPI.Endpoints.Populations
{
    public record VillagesRequest(string ServerUrl) : PopulationParameters, IServerUrlRequest;

    public class VillagesRequestValidator : Validator<VillagesRequest>
    {
        public VillagesRequestValidator()
        {
            Include(new ServerUrlRequestValidator());
            Include(new PopulationParametersValidator());
        }
    }

    public class VillagesEndpoint : Endpoint<VillagesRequest, Results<Ok<Dictionary<int, List<PopulationDto>>>, NotFound>>
    {
        private readonly GetVillagesPopulationHistoryQuery.Handler _getVillagesPopulationHistoryByParametersQuery;

        public VillagesEndpoint(GetVillagesPopulationHistoryQuery.Handler getVillagesPopulationHistoryByParametersQuery)
        {
            _getVillagesPopulationHistoryByParametersQuery = getVillagesPopulationHistoryByParametersQuery;
        }

        public override void Configure()
        {
            Get("villages");
            AllowAnonymous();
            Group<Population>();
        }

        public override async Task<Results<Ok<Dictionary<int, List<PopulationDto>>>, NotFound>> ExecuteAsync(VillagesRequest rq, CancellationToken ct)
        {
            var result = await _getVillagesPopulationHistoryByParametersQuery.HandleAsync(new(rq), ct);
            return TypedResults.Ok(result);
        }
    }
}