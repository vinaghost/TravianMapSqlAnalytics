using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Shared.Validators
{
    public class PaginationParametersValidator : AbstractValidator<IPaginationParameters>
    {
        public PaginationParametersValidator()
        {
            RuleFor(x => x.PageSize)
                .Equal(50);
        }
    }
}