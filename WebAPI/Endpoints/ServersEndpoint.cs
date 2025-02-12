using FastEndpoints;
using Features.Servers;
using Features.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Responses;

namespace WebAPI.Endpoints
{
    public record ServersRequest : GetServersByNameParameters;

    public class ServersEndpoint(IMediator mediator) : Endpoint<ServersRequest, Results<Ok<PagedListResponse<ServerDto>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Get("servers");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<PagedListResponse<ServerDto>>, NotFound>> ExecuteAsync(ServersRequest rq, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetServersByNameQuery(rq), ct);
            return TypedResults.Ok(new PagedListResponse<ServerDto>(result));
        }
    }
}