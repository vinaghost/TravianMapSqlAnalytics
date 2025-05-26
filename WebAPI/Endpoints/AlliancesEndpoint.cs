using FastEndpoints;
using Features.Dtos;
using Features.Queries.Alliances;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using WebAPI.Groups;

namespace WebAPI.Endpoints
{
    public record AlliancesRequest(string ServerUrl) : GetAlliancesByNameParameters, IServerUrlRequest;

    public class AlliancesRequestValidator : Validator<AlliancesRequest>
    {
        public AlliancesRequestValidator()
        {
            Include(new GetAllianceByNameParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    public class AlliancesEndpoint : Endpoint<AlliancesRequest, Results<Ok<PagedListResponse<AllianceDto>>, NotFound>>
    {
        private readonly GetAlliancesByNameQuery.Handler _getAlliancesByNameQuery;

        public AlliancesEndpoint(GetAlliancesByNameQuery.Handler getAlliancesByNameQuery)
        {
            _getAlliancesByNameQuery = getAlliancesByNameQuery;
        }

        public override void Configure()
        {
            Get("");
            AllowAnonymous();
            Group<Alliance>();
        }

        public override async Task<Results<Ok<PagedListResponse<AllianceDto>>, NotFound>> ExecuteAsync(AlliancesRequest rq, CancellationToken ct)
        {
            var result = await _getAlliancesByNameQuery.HandleAsync(new(rq), ct);
            return TypedResults.Ok(new PagedListResponse<AllianceDto>(result));
        }
    }
}