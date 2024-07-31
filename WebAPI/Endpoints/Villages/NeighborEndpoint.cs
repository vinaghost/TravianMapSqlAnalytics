using FastEndpoints;
using Features.Villages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Requests;

namespace WebAPI.Endpoints.Villages
{
    public record NeighborRequest(string ServerUrl) : NeighborsParameters, IServerUrlRequest;

    public class NeighborResponse()
    {
        public required IList<NeighborDto> Data { get; set; }
        public required int TotalItemCount { get; set; }
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
        public required int PageCount { get; set; }
    }

    public class NeighborRequestValidator : Validator<NeighborRequest>
    {
        public NeighborRequestValidator()
        {
            Include(new NeighborsParametersValidator());
        }
    }

    [HttpGet("/villages/neighbors"), AllowAnonymous]
    public class NeighborEndpoint(IMediator mediator) :
        Endpoint<NeighborRequest,
                Results<Ok<NeighborResponse>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<NeighborResponse>,
            NotFound>>
            ExecuteAsync(NeighborRequest rq, CancellationToken ct)
        {
            var data = await _mediator.Send(new GetNeighborsQuery(rq), ct);
            if (data is null) return TypedResults.NotFound();

            return TypedResults.Ok(new NeighborResponse()
            {
                Data = [.. data],
                TotalItemCount = data.TotalItemCount,
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                PageCount = data.PageCount
            });
        }
    }
}