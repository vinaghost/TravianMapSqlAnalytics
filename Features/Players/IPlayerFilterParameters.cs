using System.Text;

using Dapper;

using FluentValidation;

namespace Features.Players
{
    public interface IPlayerFilterParameters
    {
        int MinPlayerPopulation { get; }
        int MaxPlayerPopulation { get; }

        IList<int>? Alliances { get; }
        IList<int>? ExcludeAlliances { get; }

        IList<int>? Players { get; }
        IList<int>? ExcludePlayers { get; }
    }

    public static class PlayerFilterParametersExtensions
    {
        public static void PlayerFilterKey(this IPlayerFilterParameters parameters, StringBuilder sb)
        {
            const char SEPARATOR = '_';

            sb.Append(parameters.MinPlayerPopulation);
            sb.Append(SEPARATOR);
            sb.Append(parameters.MaxPlayerPopulation);

            sb.Append(SEPARATOR);
            if (parameters.Alliances is not null && parameters.Alliances.Count > 0)
            {
                sb.AppendJoin(',', parameters.Alliances.Distinct().Order());
            }

            sb.Append(SEPARATOR);
            if (parameters.ExcludeAlliances is not null && parameters.ExcludeAlliances.Count > 0)
            {
                sb.AppendJoin(',', parameters.ExcludeAlliances.Distinct().Order());
            }

            sb.Append(SEPARATOR);
            if (parameters.Players is not null && parameters.Players.Count > 0)
            {
                sb.AppendJoin(',', parameters.Players.Distinct().Order());
            }

            sb.Append(SEPARATOR);
            if (parameters.ExcludePlayers is not null && parameters.ExcludePlayers.Count > 0)
            {
                sb.AppendJoin(',', parameters.ExcludePlayers.Distinct().Order());
            }
        }

        public static bool IsPlayerFiltered(this IPlayerFilterParameters parameters)
        {
            if (parameters.Alliances is not null && parameters.Alliances.Count > 0) return true;
            if (parameters.ExcludeAlliances is not null && parameters.ExcludeAlliances.Count > 0) return true;
            if (parameters.Players is not null && parameters.Players.Count > 0) return true;
            if (parameters.ExcludePlayers is not null && parameters.ExcludePlayers.Count > 0) return true;
            if (parameters.MaxPlayerPopulation != 0) return true;
            return false;
        }

        public static (string WhereClause, DynamicParameters Parameters) GetDapperFilter(this IPlayerFilterParameters parameters)
        {
            var conditions = new List<string>();
            var dynamicParams = new DynamicParameters();

            if (parameters.Players?.Count > 0)
            {
                conditions.Add("p.Id IN @Players");
                dynamicParams.Add("Players", parameters.Players);
            }
            else
            {
                if (parameters.ExcludePlayers?.Count > 0)
                {
                    conditions.Add("p.Id NOT IN @ExcludePlayers");
                    dynamicParams.Add("ExcludePlayers", parameters.ExcludePlayers);
                }

                if (parameters.Alliances?.Count > 0)
                {
                    conditions.Add("p.AllianceId IN @Alliances");
                    dynamicParams.Add("Alliances", parameters.Alliances);
                }
                else if (parameters.ExcludeAlliances?.Count > 0)
                {
                    conditions.Add("p.AllianceId NOT IN @ExcludeAlliances");
                    dynamicParams.Add("ExcludeAlliances", parameters.ExcludeAlliances);
                }
            }

            if (parameters.MaxPlayerPopulation != 0)
            {
                conditions.Add("p.Population >= @MinPopulation");
                conditions.Add("p.Population <= @MaxPopulation");
                dynamicParams.Add("MinPopulation", parameters.MinPlayerPopulation);
                dynamicParams.Add("MaxPopulation", parameters.MaxPlayerPopulation);
            }

            string whereClause = conditions.Count > 0 ? string.Join(" AND ", conditions) : "1=1";

            return (whereClause, dynamicParams);
        }
    }

    public class PlayerFilterParametersValidator : AbstractValidator<IPlayerFilterParameters>
    {
        public PlayerFilterParametersValidator()
        {
            RuleFor(x => x.MinPlayerPopulation)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxPlayerPopulation)
                .GreaterThanOrEqualTo(x => x.MinPlayerPopulation);
        }
    }
}