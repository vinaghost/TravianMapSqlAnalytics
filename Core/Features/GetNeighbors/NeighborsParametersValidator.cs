using Core.Features.Shared.Validators;
using FluentValidation;

namespace Core.Features.GetNeighbors
{
    public class NeighborsParametersValidator : AbstractValidator<NeighborsParameters>
    {
        public NeighborsParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new DistanceFilterParametersValidator());
            Include(new PlayerPopulationFilterParametersValidator());
            Include(new VillagePopulationFilterParametersValidator());

            RuleFor(x => x.Distance)
                .GreaterThan(0)
                .LessThanOrEqualTo(50);
        }
    }
}