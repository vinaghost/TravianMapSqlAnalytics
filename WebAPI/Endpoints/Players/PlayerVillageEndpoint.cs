using FastEndpoints;
using Features.Players;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;

namespace WebAPI.Endpoints.Players
{
    [HttpGet("/players/{Id}/villages"), AllowAnonymous]
    public class PlayerVillageEndpoint(IMediator mediator) :
        Endpoint<IdRequest,
                Results<Ok<List<VillageDto>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<List<VillageDto>>,
            NotFound>>
            ExecuteAsync(IdRequest rq, CancellationToken ct)
        {
            var villages = await _mediator.Send(new GetVillagesQuery(rq.Id), ct);
            if (villages.Count == 0) return TypedResults.NotFound();
            return TypedResults.Ok(villages);
        }
    }
}