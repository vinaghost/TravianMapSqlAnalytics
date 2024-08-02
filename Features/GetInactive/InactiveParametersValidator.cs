using Features.Shared.Validators;
using FluentValidation;

namespace Features.GetInactiveFarms
{
    public class InactiveParametersValidator : AbstractValidator<InactiveParameters>
    {
        public InactiveParametersValidator()
        {
            Include(new DistanceFilterParametersValidator());
            Include(new IPlayerFilterParametersValidator());
            Include(new VillageFilterParametersValidator());

            RuleFor(x => x.InactiveDays)
                .GreaterThanOrEqualTo(3)
                .LessThanOrEqualTo(7);
        }
    }
}