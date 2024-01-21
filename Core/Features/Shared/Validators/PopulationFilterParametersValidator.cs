using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class PopulationFilterParametersValidator : AbstractValidator<IPopulationFilterParameters>
    {
        public PopulationFilterParametersValidator()
        {
            RuleFor(x => x.MinPopulation)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MaxPopulation)
                .GreaterThanOrEqualTo(x => x.MinPopulation);
        }
    }
}