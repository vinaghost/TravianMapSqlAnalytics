using Features.Shared.Parameters;
using Infrastructure.Entities;
using LinqKit;

namespace Features.Villages
{
    public static class VillageDataQuery
    {
        public static bool IsPlayerFiltered(IPlayerFilterParameters parameters)
        {
            if (parameters.Alliances is not null && parameters.Alliances.Count > 0) return true;
            if (parameters.ExcludeAlliances is not null && parameters.ExcludeAlliances.Count > 0) return true;
            if (parameters.Players is not null && parameters.Players.Count > 0) return true;
            if (parameters.ExcludePlayers is not null && parameters.ExcludePlayers.Count > 0) return true;
            if (parameters.MaxPlayerPopulation != 0) return true;
            return false;
        }

        public static ExpressionStarter<Player> PlayerPredicate(IPlayerFilterParameters parameters)
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

        public static ExpressionStarter<Village> VillagePredicate(IVillageFilterParameters parameters, IDistanceFilterParameters distanceParameters)
        {
            var predicate = PredicateBuilder.New<Village>(true);

            if (parameters.MinVillagePopulation != 0)
            {
                predicate = predicate
                    .And(x => x.Population >= parameters.MinVillagePopulation);
            }

            if (parameters.MaxVillagePopulation != 0)
            {
                predicate = predicate
                    .And(x => x.Population <= parameters.MaxVillagePopulation);
            }

            if (parameters.Tribe != Tribe.All)
            {
                predicate = predicate
                    .And(x => x.Tribe == (int)parameters.Tribe);
            }

            switch (parameters.Capital)
            {
                case Capital.Both:
                    break;

                case Capital.OnlyCapital:
                    predicate = predicate
                        .And(x => x.IsCapital);
                    break;

                case Capital.OnlyVillage:
                    predicate = predicate
                        .And(x => !x.IsCapital);
                    break;

                default:
                    break;
            }

            if (distanceParameters.Distance != 0)
            {
                predicate = predicate
                    .And(x => CoordinatesExtenstion.Distance(distanceParameters.X, distanceParameters.Y, x.X, x.Y) <= distanceParameters.Distance * distanceParameters.Distance);
            }

            return predicate;
        }
    }
}