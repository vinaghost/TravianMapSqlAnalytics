using FastEndpoints;
using Features.Queries.Servers;
using Features.Shared.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Responses;

namespace WebAPI.Endpoints
{
    public record ServersRequest : GetServersByNameParameters;

    public class ServersEndpoint : Endpoint<ServersRequest, Results<Ok<PagedListResponse<ServerDto>>, NotFound>>
    {
        private readonly GetServersByNameQuery.Handler _getServersByNameQuery;

        public ServersEndpoint(GetServersByNameQuery.Handler getServersByNameQuery)
        {
            _getServersByNameQuery = getServersByNameQuery;
        }

        public override void Configure()
        {
            Get("servers");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<PagedListResponse<ServerDto>>, NotFound>> ExecuteAsync(ServersRequest rq, CancellationToken ct)
        {
            var result = await _getServersByNameQuery.HandleAsync(new(rq), ct);
            return TypedResults.Ok(new PagedListResponse<ServerDto>(result));
        }
    }
}