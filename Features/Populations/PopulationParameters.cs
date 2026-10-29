using System.Text;

using Features.Shared.Parameters;

using FluentValidation;

namespace Features.Populations
{
    public record PopulationParameters : IParameters
    {
        public IList<int>? Ids { get; init; }
        public int Days { get; init; }
        public string Key()
        {
            var sb = new StringBuilder();

            const char SEPARATOR = '_';

            if (Ids is not null && Ids.Count > 0)
            {
                sb.AppendJoin(',', Ids.Distinct().Order());
            }

            sb.Append(SEPARATOR);
            sb.Append(Days);

            return sb.ToString();
        }
    }

    public class PopulationFilterParametersValidator : AbstractValidator<PopulationParameters>
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