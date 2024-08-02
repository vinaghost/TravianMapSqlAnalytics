using FastEndpoints;
using Features.Villages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;

namespace WebAPI.Endpoints.Villages
{
    public record InactiveRequest(string ServerUrl) : InactiveParameters, IServerUrlRequest;

    public class InactiveResponse()
    {
        public required IList<InactiveDto> Data { get; set; }
        public required int TotalItemCount { get; set; }
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
        public required int PageCount { get; set; }
    }

    public class InactiveRequestValidator : Validator<InactiveRequest>
    {
        public InactiveRequestValidator()
        {
            Include(new InactiveParametersValidator());
        }
    }

    [HttpGet("/villages/inactives"), AllowAnonymous]
    public class InactiveEndpoint(IMediator mediator) :
        Endpoint<InactiveRequest,
                Results<Ok<InactiveResponse>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<InactiveResponse>,
            NotFound>>
            ExecuteAsync(InactiveRequest rq, CancellationToken ct)
        {
            var data = await _mediator.Send(new GetInactiveQuery(rq), ct);

            return TypedResults.Ok(new InactiveResponse()
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