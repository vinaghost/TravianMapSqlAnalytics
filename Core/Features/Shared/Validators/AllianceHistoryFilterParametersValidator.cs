using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class AllianceHistoryFilterParametersValidator : AbstractValidator<IAllianceHistoryFilterParameters>
    {
        public AllianceHistoryFilterParametersValidator()
        {
            Include(new HistoryParametersValidator());

            RuleFor(x => x.MinChangeAlliance)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MaxChangeAlliance)
                .GreaterThanOrEqualTo(x => x.MinChangeAlliance);
        }
    }
}