using FastEndpoints;
using Features.Populations;
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

    public class PlayersEndpoint : Endpoint<PlayersRequest, Results<Ok<Dictionary<int, List<PopulationDto>>>, NotFound>>
    {
        private readonly GetPlayersPopulationHistoryQuery.Handler _getPlayersPopulationHistoryByParametersQuery;

        public PlayersEndpoint(GetPlayersPopulationHistoryQuery.Handler getPlayersPopulationHistoryByParametersQuery)
        {
            _getPlayersPopulationHistoryByParametersQuery = getPlayersPopulationHistoryByParametersQuery;
        }

        public override void Configure()
        {
            Get("players");
            AllowAnonymous();
            Group<Population>();
        }

        public override async Task<Results<Ok<Dictionary<int, List<PopulationDto>>>, NotFound>> ExecuteAsync(PlayersRequest rq, CancellationToken ct)
        {
            var result = await _getPlayersPopulationHistoryByParametersQuery.HandleAsync(new(rq), ct);
            return TypedResults.Ok(result);
        }
    }
}