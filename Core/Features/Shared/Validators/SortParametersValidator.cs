using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class SortParametersValidator : AbstractValidator<ISortParameters>
    {
        public SortParametersValidator()
        {
            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(1);
        }
    }
}