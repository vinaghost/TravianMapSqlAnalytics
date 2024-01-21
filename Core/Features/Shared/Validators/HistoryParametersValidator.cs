using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class HistoryParametersValidator : AbstractValidator<IHistoryParameters>
    {
        public HistoryParametersValidator()
        {
            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.Today)
                .GreaterThanOrEqualTo(DateTime.Today.AddDays(-14));
        }
    }
}