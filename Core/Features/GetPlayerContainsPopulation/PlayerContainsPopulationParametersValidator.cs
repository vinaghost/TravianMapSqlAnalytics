using Core.Features.Shared.Validators;
using FluentValidation;

namespace Core.Features.GetPlayerContainsPopulation
{
    public class PlayerContainsPopulationParametersValidator : AbstractValidator<PlayerContainsPopulationParameters>
    {
        public PlayerContainsPopulationParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new PlayerFilterParametersValidator());
            Include(new SortParametersValidator());
        }
    }
}