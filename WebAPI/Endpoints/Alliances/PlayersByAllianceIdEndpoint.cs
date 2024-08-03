using FastEndpoints;
using Features.Players;
using Features.Shared.Parameters;
using Features.Shared.Validators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using X.PagedList;

namespace WebAPI.Endpoints.Alliances
{
    public record PlayersByAllianceIdRequest(string ServerUrl, int Id, int PageNumber, int PageSize) : IServerUrlRequest, IIdRequest, IPaginationParameters;

    public class PlayersByAllianceIdRequestValidator : Validator<PlayersByAllianceIdRequest>
    {
        public PlayersByAllianceIdRequestValidator()
        {
            Include(new IdRequestValidator());
            Include(new PaginationParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    public class PlayersByAllianceIdRequestResponse(IPagedList<PlayerDto> pagedList) : PagedListResponse<PlayerDto>(pagedList);

    [HttpGet("/alliances/{Id}/players"), AllowAnonymous]
    public class PlayersByAllianceIdEndpoint(IMediator mediator) :
        Endpoint<PlayersByAllianceIdRequest,
                Results<Ok<PlayersByAllianceIdRequestResponse>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<PlayersByAllianceIdRequestResponse>, NotFound>>
            ExecuteAsync(PlayersByAllianceIdRequest rq, CancellationToken ct)
        {
            var parameters = new GetPlayersParameters
            {
                PageNumber = rq.PageNumber,
                PageSize = rq.PageSize,
                Alliances = [rq.Id]
            };

            var players = await _mediator.Send(new GetPlayersQuery(parameters), ct);
            return TypedResults.Ok(new PlayersByAllianceIdRequestResponse(players));
        }
    }
}