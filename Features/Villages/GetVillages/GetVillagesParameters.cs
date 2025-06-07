using Features.Players;
using Features.Shared.Parameters;
using System.Text;

namespace Features.Villages.GetVillages
{
    public record GetVillagesParameters : IPaginationParameters, IPlayerFilterParameters, IVillageFilterParameters, IDistanceFilterParameters
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public int X { get; init; }
        public int Y { get; init; }

        public int Distance { get; init; }

        public int MinPlayerPopulation { get; init; }
        public int MaxPlayerPopulation { get; init; }

        public int MinVillagePopulation { get; init; }
        public int MaxVillagePopulation { get; init; }

        public Capital Capital { get; init; }
        public Tribe Tribe { get; init; }

        public IList<int>? Alliances { get; init; }
        public IList<int>? ExcludeAlliances { get; init; }

        public IList<int>? Players { get; init; }
        public IList<int>? ExcludePlayers { get; init; }
    }

    public static class GetVillagesParametersExtension
    {
        private static string GenerateKey(this GetVillagesParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            parameters.PaginationKey(sb);
            sb.Append(SEPARATOR);
            parameters.PlayerFilterKey(sb);
            sb.Append(SEPARATOR);
            parameters.VillageFilterKey(sb);
            sb.Append(SEPARATOR);
            parameters.DistanceFilterKey(sb);
            return sb.ToString();
        }

        public static string Key(this GetVillagesParameters parameters)
        {
            return parameters.GenerateKey();
        }

        public static string KeyParent(this GetVillagesParameters parameters)
        {
            return parameters.GenerateKey();
        }
    }
}