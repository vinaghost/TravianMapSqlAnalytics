using Core.Features.Shared.Parameters;
using System.Text;

namespace Core.Features.GetInactiveFarms
{
    public record InactiveFarmParameters : IPaginationParameters, IDistanceFilterParameters, IPlayerPopulationFilterParameters, IVillagePopulationFilterParameters
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

        public IList<int> Alliances { get; set; } = [];
        public IList<int> ExcludeAlliances { get; set; } = [];
    }

    public static class InactiveVillagesParametersExtension
    {
        public static string Key(this InactiveFarmParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';
            sb.Append(parameters.X);
            sb.Append(SEPARATOR);
            sb.Append(parameters.Y);
            sb.Append(SEPARATOR);
            sb.Append(parameters.Date.ToString("d"));
            sb.Append(SEPARATOR);
            sb.Append(parameters.PageNumber);
            sb.Append(SEPARATOR);
            sb.Append(parameters.PageSize);
            if (parameters.Alliances.Count > 0)
            {
                sb.Append(SEPARATOR);
                sb.AppendJoin(',', parameters.Alliances.Distinct().Order());
            }
            else if (parameters.ExcludeAlliances.Count > 0)
            {
                sb.Append(SEPARATOR);
                sb.Append(SEPARATOR);
                sb.AppendJoin(',', parameters.ExcludeAlliances.Distinct().Order());
            }

            return sb.ToString();
        }
    }
}