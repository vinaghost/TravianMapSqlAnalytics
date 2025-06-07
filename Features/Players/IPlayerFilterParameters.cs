using FluentValidation;
using Infrastructure.Entities;
using LinqKit;
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

        public static bool IsPlayerFiltered(this IPlayerFilterParameters parameters)
        {
            if (parameters.Alliances is not null && parameters.Alliances.Count > 0) return true;
            if (parameters.ExcludeAlliances is not null && parameters.ExcludeAlliances.Count > 0) return true;
            if (parameters.Players is not null && parameters.Players.Count > 0) return true;
            if (parameters.ExcludePlayers is not null && parameters.ExcludePlayers.Count > 0) return true;
            if (parameters.MaxPlayerPopulation != 0) return true;
            return false;
        }

        public static ExpressionStarter<Player> GetPredicate(this IPlayerFilterParameters parameters)
        {
            var predicate = PredicateBuilder.New<Player>(true);

            if (parameters.Players is not null && parameters.Players.Count > 0)
            {
                if (parameters.Players.Count == 1)
                {
                    predicate = predicate
                        .And(x => x.Id == parameters.Players[0]);
                }
                else
                {
                    predicate = predicate
                        .And(x => parameters.Players.Contains(x.Id));
                }
            }
            else
            {
                if (parameters.ExcludePlayers is not null && parameters.ExcludePlayers.Count > 0)
                {
                    if (parameters.ExcludePlayers.Count == 1)
                    {
                        predicate = predicate
                            .And(x => x.Id != parameters.ExcludePlayers[0]);
                    }
                    else
                    {
                        predicate = predicate
                            .And(x => !parameters.ExcludePlayers.Contains(x.Id));
                    }
                }

                if (parameters.Alliances is not null && parameters.Alliances.Count > 0)
                {
                    if (parameters.Alliances.Count == 1)
                    {
                        predicate = predicate
                            .And(x => x.AllianceId == parameters.Alliances[0]);
                    }
                    else
                    {
                        predicate = predicate
                            .And(x => parameters.Alliances.Contains(x.AllianceId));
                    }
                }
                else
                {
                    if (parameters.ExcludeAlliances is not null && parameters.ExcludeAlliances.Count > 0)
                    {
                        if (parameters.ExcludeAlliances.Count == 1)
                        {
                            predicate = predicate
                                .And(x => x.AllianceId != parameters.ExcludeAlliances[0]);
                        }
                        else
                        {
                            predicate = predicate
                                .And(x => !parameters.ExcludeAlliances.Contains(x.AllianceId));
                        }
                    }
                }
            }

            if (parameters.MaxPlayerPopulation != 0)
            {
                predicate = predicate
                    .And(x => x.Population >= parameters.MinPlayerPopulation)
                    .And(x => x.Population <= parameters.MaxPlayerPopulation);
            }

            return predicate;
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