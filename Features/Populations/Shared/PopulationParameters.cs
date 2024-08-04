using Features.Shared.Parameters;
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

            parameters.PopulationFilterKey(sb);

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