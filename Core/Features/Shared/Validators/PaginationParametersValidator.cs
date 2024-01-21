using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
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