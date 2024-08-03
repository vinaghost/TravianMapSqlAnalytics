using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Shared.Validators
{
    public class PopulationFilterParametersValidator : AbstractValidator<IPopulationFilterParmeters>
    {
        public PopulationFilterParametersValidator()
        {
            RuleFor(x => x.Ids)
                .NotEmpty();
            RuleFor(x => x.Days)
                .GreaterThan(0);
        }
    }
}