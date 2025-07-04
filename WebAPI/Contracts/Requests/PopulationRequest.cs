﻿using FastEndpoints;
using Features.Populations;

namespace WebAPI.Contracts.Requests
{
    public record PopulationRequest(string ServerUrl) : PopulationParameters, IServerUrlRequest;

    public class PopulationRequestValidator : Validator<PopulationRequest>
    {
        public PopulationRequestValidator()
        {
            Include(new PopulationParametersValidator());
            Include(new ServerUrlRequestValidator());
        }
    }
}