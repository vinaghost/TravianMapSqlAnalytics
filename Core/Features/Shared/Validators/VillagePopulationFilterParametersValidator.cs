using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class VillagePopulationFilterParametersValidator : AbstractValidator<IVillagePopulationFilterParameters>
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