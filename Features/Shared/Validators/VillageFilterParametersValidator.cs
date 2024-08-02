using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Shared.Validators
{
    public class VillageFilterParametersValidator : AbstractValidator<IVillageFilterParameters>
    {
        public VillageFilterParametersValidator()
        {
            RuleFor(x => x.MinVillagePopulation)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxVillagePopulation)
                .GreaterThanOrEqualTo(x => x.MinVillagePopulation);
        }
    }
}