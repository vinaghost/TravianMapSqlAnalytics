using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Shared.Validators
{
    public class PlayerFilterParametersValidator : AbstractValidator<IPlayerFilterParameters>
    {
        public PlayerFilterParametersValidator()
        {
            RuleFor(x => x.MinPlayerPopulation)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxPlayerPopulation)
                .GreaterThanOrEqualTo(x => x.MinPlayerPopulation);
        }
    }
}