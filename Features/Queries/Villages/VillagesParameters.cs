using Features.Enums;
using Features.Parameters;
using FluentValidation;
using System.Text;

namespace Features.Queries.Villages
{
    public record VillagesParameters : IPaginationParameters, IPlayerFilterParameters, IVillageFilterParameters, IDistanceFilterParameters
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
        private static string GenerateKey(this VillagesParameters parameters)
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

        public static string Key(this VillagesParameters parameters)
        {
            return parameters.GenerateKey();
        }

        public static string KeyParent(this VillagesParameters parameters)
        {
            return parameters.GenerateKey();
        }
    }

    public class VillagesParametersValidator : AbstractValidator<VillagesParameters>
    {
        public VillagesParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new DistanceFilterParametersValidator());
            Include(new PlayerFilterParametersValidator());
            Include(new VillageFilterParametersValidator());
        }
    }
}