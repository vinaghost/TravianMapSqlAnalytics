using FluentValidation;
using Infrastructure.Entities;
using LinqKit;
using System.Text;

namespace Features.Villages
{
    public interface IVillageFilterParameters
    {
        int MinVillagePopulation { get; }
        int MaxVillagePopulation { get; }

        Capital Capital { get; }

        Tribe Tribe { get; }
    }

    public static class IVillageFilterParametersExtensions
    {
        public static void VillageFilterKey(this IVillageFilterParameters parameters, StringBuilder sb)
        {
            const char SEPARATOR = '_';

            sb.Append(parameters.MinVillagePopulation);
            sb.Append(SEPARATOR);
            sb.Append(parameters.MaxVillagePopulation);
            sb.Append(SEPARATOR);
            sb.Append(parameters.Capital);
            sb.Append(SEPARATOR);
            sb.Append(parameters.Tribe);
        }

        public static ExpressionStarter<Village> GetPredicate(this IVillageFilterParameters parameters)
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
            return predicate;
        }
    }

    public class VillageFilterParametersValidator : AbstractValidator<IVillageFilterParameters>
    {
        public VillageFilterParametersValidator()
        {
            RuleFor(x => x.MinVillagePopulation)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxVillagePopulation)
                .GreaterThanOrEqualTo(x => x.MinVillagePopulation);

            RuleFor(x => x.Capital)
                .IsInEnum();

            RuleFor(x => x.Tribe)
                .IsInEnum();
        }
    }
}