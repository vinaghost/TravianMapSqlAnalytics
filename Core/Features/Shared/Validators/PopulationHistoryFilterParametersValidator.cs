using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class PopulationHistoryFilterParametersValidator : AbstractValidator<IPopulationHistoryFilterParameters>
    {
        public PopulationHistoryFilterParametersValidator()
        {
            Include(new HistoryParametersValidator());

            RuleFor(x => x.MinChangePopulation)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MaxChangePopulation)
                .GreaterThanOrEqualTo(x => x.MinChangePopulation);
        }
    }
}