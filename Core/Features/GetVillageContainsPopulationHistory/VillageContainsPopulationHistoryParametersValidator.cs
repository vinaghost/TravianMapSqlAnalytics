using Core.Features.Shared.Validators;
using FluentValidation;

namespace Core.Features.GetVillageContainsPopulationHistory
{
    public class VillageContainsPopulationHistoryParametersValidator : AbstractValidator<VillageContainsPopulationHistoryParameters>
    {
        public VillageContainsPopulationHistoryParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new VillageFilterParametersValidator());
            Include(new PopulationHistoryFilterParametersValidator());
            Include(new SortParametersValidator());
        }
    }
}