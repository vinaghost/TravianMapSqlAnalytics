using FastEndpoints;
using Features.Queries.Villages.ByPlayerId;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Groups;

namespace WebAPI.Endpoints.Players
{
    public record VillagesRequest(string ServerUrl, int Id) : IServerUrlRequest, IIdRequest;

    public class VillagePopulationRequestValidator : Validator<VillagesRequest>
    {
        public VillagePopulationRequestValidator()
        {
            Include(new ServerUrlRequestValidator());
            Include(new IdRequestValidator());
        }
    }

    public class VillagesEndpoint : Endpoint<VillagesRequest, Results<Ok<IList<VillageDto>>, NotFound>>
    {
        private readonly GetVillagesByPlayerIdQuery.Handler _getVillagesByPlayerIdQuery;

        public VillagesEndpoint(GetVillagesByPlayerIdQuery.Handler getVillagesByPlayerIdQuery)
        {
            _getVillagesByPlayerIdQuery = getVillagesByPlayerIdQuery;
        }

        public override void Configure()
        {
            Get("{Id}/villages");
            AllowAnonymous();
            Group<Player>();
        }

        public override async Task<Results<Ok<IList<VillageDto>>, NotFound>> ExecuteAsync(VillagesRequest rq, CancellationToken ct)
        {
            var result = await _getVillagesByPlayerIdQuery.HandleAsync(new(rq.Id), ct);
            return TypedResults.Ok(result);
        }
    }
}