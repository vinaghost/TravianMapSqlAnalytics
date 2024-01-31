using Core.Features.Shared.Validators;
using FluentValidation;

namespace Core.Features.GetInactiveVillage
{
    public class InactiveVillagesParametersValidator : AbstractValidator<InactiveVillagesParameters>
    {
        public InactiveVillagesParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new DistanceFilterParametersValidator());
            Include(new PlayerPopulationFilterParametersValidator());
            Include(new VillagePopulationFilterParametersValidator());

            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.Now)
                .GreaterThanOrEqualTo(DateTime.Now.AddDays(-14));
        }
    }
}