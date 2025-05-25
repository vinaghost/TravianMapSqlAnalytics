using FastEndpoints;
using Features.Populations;
using Features.Queries.Populations.Shared;
using Features.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Groups;

namespace WebAPI.Endpoints.Populations
{
    public record PlayersRequest(string ServerUrl) : PopulationParameters, IServerUrlRequest;

    public class PlayersRequestValidator : Validator<PlayersRequest>
    {
        public PlayersRequestValidator()
        {
            Include(new ServerUrlRequestValidator());
            Include(new PopulationParametersValidator());
        }
    }

    public class PlayersEndpoint(IMediator mediator) : Endpoint<PlayersRequest, Results<Ok<Dictionary<int, List<PopulationDto>>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Get("players");
            AllowAnonymous();
            Group<Population>();
        }

        public override async Task<Results<Ok<Dictionary<int, List<PopulationDto>>>, NotFound>> ExecuteAsync(PlayersRequest rq, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetPlayersPopulationHistoryByParametersQuery(rq), ct);
            return TypedResults.Ok(result);
        }
    }
}