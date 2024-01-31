using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class DistanceFilterParametersValidator : AbstractValidator<IDistanceFilterParameters>
    {
        public DistanceFilterParametersValidator()
        {
            RuleFor(x => x.X)
                .LessThanOrEqualTo(200)
                .GreaterThanOrEqualTo(-200);

            RuleFor(x => x.Y)
                .LessThanOrEqualTo(200)
                .GreaterThanOrEqualTo(-200);

            RuleFor(x => x.MinDistance)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxDistance)
                .GreaterThanOrEqualTo(x => x.MinDistance);
        }
    }
}