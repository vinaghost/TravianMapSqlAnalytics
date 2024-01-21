using Core.Features.Shared.Validators;
using FluentValidation;

namespace Core.Features.GetVillageContainsDistance
{
    public class VillageContainsDistanceParametersValidator : AbstractValidator<VillageContainsDistanceParameters>
    {
        public VillageContainsDistanceParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new VillageFilterParametersValidator());
            Include(new DistanceFilterParametersValidator());
            Include(new SortParametersValidator());
        }
    }
}