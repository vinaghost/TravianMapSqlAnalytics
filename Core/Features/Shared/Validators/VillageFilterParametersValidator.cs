using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class VillageFilterParametersValidator : AbstractValidator<IVillageFilterParameters>
    {
        public VillageFilterParametersValidator()
        {
            RuleFor(x => x.Tribe)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(8)
                .NotEqual(4);
        }
    }
}