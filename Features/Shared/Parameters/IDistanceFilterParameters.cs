using System.Text;

namespace Features.Shared.Parameters
{
    public interface IDistanceFilterParameters
    {
        public int X { get; }
        public int Y { get; }

        public int Distance { get; }
    }

    public static class IDistanceFilterParametersExtensions
    {
        public static void DistanceFilterKey(this IDistanceFilterParameters parameters, StringBuilder sb)
        {
            const char SEPARATOR = '_';

            sb.Append(parameters.X);
            sb.Append(SEPARATOR);
            sb.Append(parameters.Y);
            sb.Append(SEPARATOR);
            sb.Append(parameters.Distance);
        }
    }

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