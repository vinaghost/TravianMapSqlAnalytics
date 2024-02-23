using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Shared.Validators
{
    public class PlayerPopulationFilterParametersValidator : AbstractValidator<IPlayerFilterParameters>
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