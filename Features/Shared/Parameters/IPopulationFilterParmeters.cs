using FluentValidation;
using System.Text;

namespace Features.Shared.Parameters
{
    public interface IPopulationFilterParmeters
    {
        IList<int>? Ids { get; }

        int Days { get; }
    }

    public static class PopulationFilterParametersExtensions
    {
        public static void PopulationFilterKey(this IPopulationFilterParmeters parameters, StringBuilder sb)
        {
            const char SEPARATOR = '_';

            if (parameters.Ids is not null && parameters.Ids.Count > 0)
            {
                sb.AppendJoin(',', parameters.Ids.Distinct().Order());
            }

            sb.Append(SEPARATOR);
            sb.Append(parameters.Days);
        }
    }

    public class PopulationFilterParametersValidator : AbstractValidator<IPopulationFilterParmeters>
    {
        public PopulationFilterParametersValidator()
        {
            RuleFor(x => x.Ids)
                .NotEmpty();
            RuleFor(x => x.Days)
                .GreaterThan(0);
        }
    }
}