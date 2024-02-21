using Core.Features.Shared.Validators;
using FluentValidation;

namespace Core.Features.GetInactiveFarms
{
    public class InactiveFarmParametersValidator : AbstractValidator<InactiveFarmParameters>
    {
        public InactiveFarmParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new DistanceFilterParametersValidator());
            Include(new PlayerPopulationFilterParametersValidator());
            Include(new VillagePopulationFilterParametersValidator());
        }
    }
}