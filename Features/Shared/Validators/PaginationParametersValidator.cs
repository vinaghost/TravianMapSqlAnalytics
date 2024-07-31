using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Shared.Validators
{
    public class PaginationParametersValidator : AbstractValidator<IPaginationParameters>
    {
        public PaginationParametersValidator()
        {
            RuleFor(x => x.PageSize)
                .NotEmpty()
                .GreaterThanOrEqualTo(10);

            RuleFor(x => x.PageNumber)
                .NotEmpty()
                .GreaterThanOrEqualTo(1);
        }
    }
}