using Features.Shared.Validators;
using FluentValidation;

namespace Features.GetInactiveFarms
{
    public class InactiveParametersValidator : AbstractValidator<InactiveParameters>
    {
        public InactiveParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new DistanceFilterParametersValidator());
            Include(new PlayerPopulationFilterParametersValidator());
            Include(new VillagePopulationFilterParametersValidator());

            RuleFor(x => x.InactiveDays)
                .GreaterThanOrEqualTo(3)
                .LessThanOrEqualTo(7);
        }
    }
}