using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class DistanceFilterParametersValidator : AbstractValidator<IDistanceFilterParameters>
    {
        public DistanceFilterParametersValidator()
        {
            RuleFor(x => x.TargetX)
                .LessThanOrEqualTo(200)
                .GreaterThanOrEqualTo(-200);

            RuleFor(x => x.TargetY)
                .LessThanOrEqualTo(200)
                .GreaterThanOrEqualTo(-200);

            RuleFor(x => x.MinDistance)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxDistance)
                .LessThanOrEqualTo(x => x.MinDistance);
        }
    }
}