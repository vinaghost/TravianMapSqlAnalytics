using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Shared.Validators
{
    public class VillagePopulationFilterParametersValidator : AbstractValidator<IVillageFilterParameters>
    {
        public VillagePopulationFilterParametersValidator()
        {
            RuleFor(x => x.MinVillagePopulation)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxVillagePopulation)
                .GreaterThanOrEqualTo(x => x.MinVillagePopulation);
        }
    }
}