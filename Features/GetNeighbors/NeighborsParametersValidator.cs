using Features.Shared.Validators;
using FluentValidation;

namespace Features.GetNeighbors
{
    public class NeighborsParametersValidator : AbstractValidator<NeighborsParameters>
    {
        public NeighborsParametersValidator()
        {
            Include(new DistanceFilterParametersValidator());
            Include(new PlayerPopulationFilterParametersValidator());
            Include(new VillagePopulationFilterParametersValidator());
        }
    }
}