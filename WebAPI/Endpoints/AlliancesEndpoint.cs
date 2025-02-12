﻿using FastEndpoints;
using Features.Alliances;
using Features.Shared.Dtos;
using MediatR;
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

    public class AlliancesEndpoint(IMediator mediator) : Endpoint<AlliancesRequest, Results<Ok<PagedListResponse<AllianceDto>>, NotFound>>
    {
        private readonly IMediator _mediator = mediator;

        public override void Configure()
        {
            Get("");
            AllowAnonymous();
            Group<Alliance>();
        }

        public override async Task<Results<Ok<PagedListResponse<AllianceDto>>, NotFound>> ExecuteAsync(AlliancesRequest rq, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAlliancesByNameQuery(rq), ct);
            return TypedResults.Ok(new PagedListResponse<AllianceDto>(result));
        }
    }
}