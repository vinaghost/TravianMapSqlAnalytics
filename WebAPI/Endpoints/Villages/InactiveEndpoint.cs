﻿using FastEndpoints;
using Features.Shared.Dtos;
using Features.Villages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Contracts.Requests;
using WebAPI.Contracts.Responses;
using X.PagedList;

namespace WebAPI.Endpoints.Villages
{
    public record InactiveRequest(string ServerUrl) : GetInactiveParameters, IServerUrlRequest;

    public class InactiveRequestValidator : Validator<InactiveRequest>
    {
        public InactiveRequestValidator()
        {
            Include(new GetInactiveParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }

    public class InactiveResponse(IPagedList<VillageDto> pagedList) : PagedListResponse<VillageDto>(pagedList);

    [HttpGet("/villages/inactives"), AllowAnonymous]
    public class InactiveEndpoint(IMediator mediator) :
        Endpoint<InactiveRequest,
                Results<Ok<InactiveResponse>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override async Task<Results<
            Ok<InactiveResponse>, NotFound>>
            ExecuteAsync(InactiveRequest rq, CancellationToken ct)
        {
            var inactives = await _mediator.Send(new GetInactiveQuery(rq), ct);

            return TypedResults.Ok(new InactiveResponse(inactives));
        }
    }
}