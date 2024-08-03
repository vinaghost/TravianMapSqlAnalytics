using Features.Shared.Parameters;
using Features.Shared.Validators;
using FluentValidation;
using System.Text;

namespace Features.Populations.Shared
{
    public record PopulationParameters : IPopulationFilterParmeters
    {
        public IList<int>? Ids { get; init; }
        public int Days { get; init; }
    }

    public static class PopulationParametersExtension
    {
        public static string Key(this PopulationParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            sb.Append(parameters.Days);
            if (parameters.Ids is not null && parameters.Ids.Count > 0)
            {
                sb.Append(SEPARATOR);
                sb.Append(string.Join(',', parameters.Ids.Distinct().Order()));
            }

            return sb.ToString();
        }
    }

    public class PopulationParametersValidator : AbstractValidator<PopulationParameters>
    {
        public PopulationParametersValidator()
        {
            Include(new PopulationFilterParametersValidator());
        }
    }
}