using Core.Features.Shared.Parameters;

namespace Core.Features.GetInactiveVillage
{
    public record InactiveVillagesParameters : IPaginationParameters, IDistanceFilterParameters, IPlayerPopulationFilterParameters, IVillagePopulationFilterParameters
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;

        public int PageSize { get; set; } = DefaultParameters.PageSize;

        public int X { get; set; }
        public int Y { get; set; }
        public DateTime Date { get; set; } = DefaultParameters.Date;

        public int MinDistance { get; set; }
        public int MaxDistance { get; set; }

        public int MinPlayerPopulation { get; set; }
        public int MaxPlayerPopulation { get; set; }

        public int MinVillagePopulation { get; set; }
        public int MaxVillagePopulation { get; set; }
    }

    public static class InactiveVillagesParametersExtension
    {
        public static string Key(this InactiveVillagesParameters parameters)
        {
            return $"{parameters.X}_{parameters.Y}_{parameters.Date:d}_{parameters.PageNumber}_{parameters.PageSize}";
        }
    }
}