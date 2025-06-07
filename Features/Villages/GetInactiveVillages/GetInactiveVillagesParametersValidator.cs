using Features.Villages.GetVillages;
using FluentValidation;

namespace Features.Villages.GetInactiveVillages
{
    public class GetInactiveVillagesParametersValidator : AbstractValidator<GetInactiveVillagesParameters>
    {
        public GetInactiveVillagesParametersValidator()
        {
            Include(new GetVillagesParametersValidator());

            RuleFor(x => x.InactiveDays)
                .NotEmpty()
                .GreaterThanOrEqualTo(3);
        }
    }
}