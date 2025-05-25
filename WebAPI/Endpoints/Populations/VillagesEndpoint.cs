using FastEndpoints;
using Features.Queries.Populations;
using Features.Queries.Populations.Shared;
using Features.Shared.Dtos;
using MediatR;
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

    public class VillagesEndpoint(IMediator mediator) : Endpoint<VillagesRequest, Results<Ok<Dictionary<int, List<PopulationDto>>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Get("villages");
            AllowAnonymous();
            Group<Population>();
        }

        public override async Task<Results<Ok<Dictionary<int, List<PopulationDto>>>, NotFound>> ExecuteAsync(VillagesRequest rq, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetVillagesPopulationHistoryByParametersQuery(rq), ct);
            return TypedResults.Ok(result);
        }
    }
}