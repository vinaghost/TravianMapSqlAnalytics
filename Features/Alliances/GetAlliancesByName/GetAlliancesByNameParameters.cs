using Features.Shared.Parameters;
using System.Text;

namespace Features.Alliances.GetAlliancesByName
{
    public record GetAlliancesByNameParameters : IPaginationParameters, ISearchTermParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? SearchTerm { get; init; }
    }

    public static class GetAlliancesByNameParametersExtension
    {
        public static string Key(this GetAlliancesByNameParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            parameters.PaginationKey(sb);
            sb.Append(SEPARATOR);
            parameters.SearchTermKey(sb);

            return sb.ToString();
        }
    }
}