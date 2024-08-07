﻿using FastEndpoints;
using Features.Players;
using Features.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using X.PagedList;

namespace WebAPI.Endpoints.Players
{
    public record PlayersRequest(string ServerUrl) : GetPlayersParameters, IServerUrlRequest;

    public class PlayersRequestValidator : Validator<PlayersRequest>
    {
        public PlayersRequestValidator()
        {
            Include(new GetPlayersParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    public class PlayersResponse(IPagedList<PlayerDto> pagedList) : PagedListResponse<PlayerDto>(pagedList);

    [HttpGet("/players/"), AllowAnonymous]
    public class PlayersEnpoint(IMediator mediator) :
       Endpoint<PlayersRequest,
               Results<Ok<PlayersResponse>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<PlayersResponse>, NotFound>>
            ExecuteAsync(PlayersRequest rq, CancellationToken ct)
        {
            var players = await _mediator.Send(new GetPlayersQuery(rq), ct);
            return TypedResults.Ok(new PlayersResponse(players));
        }
    }
}