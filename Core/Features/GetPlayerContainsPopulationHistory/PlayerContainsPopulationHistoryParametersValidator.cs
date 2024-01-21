using Core.Features.Shared.Validators;
using FluentValidation;

namespace Core.Features.GetPlayerContainsPopulationHistory
{
    public class PlayerContainsPopulationHistoryParametersValidator : AbstractValidator<PlayerContainsPopulationHistoryParameters>
    {
        public PlayerContainsPopulationHistoryParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new PlayerFilterParametersValidator());
            Include(new PopulationHistoryFilterParametersValidator());
            Include(new SortParametersValidator());
        }
    }
}