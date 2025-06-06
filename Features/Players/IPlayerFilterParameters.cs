using FluentValidation;
using System.Text;

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