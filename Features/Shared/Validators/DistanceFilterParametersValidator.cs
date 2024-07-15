using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Shared.Validators
{
    public class DistanceFilterParametersValidator : AbstractValidator<IDistanceFilterParameters>
    {
        public DistanceFilterParametersValidator()
        {
            RuleFor(x => x.X)
                .GreaterThanOrEqualTo(-200)
                .LessThanOrEqualTo(200);

            RuleFor(x => x.Y)
                .GreaterThanOrEqualTo(-200)
                .LessThanOrEqualTo(200);

            RuleFor(x => x.Distance)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(300);
        }
    }
}