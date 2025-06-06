using Features.Shared.Parameters;
using System.Text;

namespace Features.Players.GetPlayersByName
{
    public record GetPlayersByNameParameters : IPaginationParameters, ISearchTermParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? SearchTerm { get; init; }
    }

    public static class GetPlayersByNameParametersExtension
    {
        public static string Key(this GetPlayersByNameParameters parameters)
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