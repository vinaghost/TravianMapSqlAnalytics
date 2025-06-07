using System.Text;

namespace Features.Populations
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
}