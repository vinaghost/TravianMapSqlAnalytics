using Features.Shared.Validators;
using FluentValidation;

namespace Features.GetInactiveFarms
{
    public class InactiveFarmParametersValidator : AbstractValidator<InactiveFarmParameters>
    {
        public InactiveFarmParametersValidator()
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