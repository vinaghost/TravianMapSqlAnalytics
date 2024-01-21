using Core.Features.Shared.Validators;
using FluentValidation;

namespace Core.Features.GetPlayerContainsAllianceHistory
{
    public class PlayerContainsAllianceHistoryParametersValidator : AbstractValidator<PlayerContainsAllianceHistoryParameters>
    {
        public PlayerContainsAllianceHistoryParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new PlayerFilterParametersValidator());
            Include(new AllianceHistoryFilterParametersValidator());
            Include(new SortParametersValidator());
        }
    }
}