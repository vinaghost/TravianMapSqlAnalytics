﻿using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class PlayerPopulationFilterParametersValidator : AbstractValidator<IPlayerPopulationFilterParameters>
    {
        public PlayerPopulationFilterParametersValidator()
        {
            RuleFor(x => x.MinPlayerPopulation)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxPlayerPopulation)
                .GreaterThanOrEqualTo(x => x.MinPlayerPopulation);
        }
    }
}