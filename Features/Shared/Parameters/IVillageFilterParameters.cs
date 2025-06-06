using Features.Villages;
using FluentValidation;
using System.Text;

namespace Features.Shared.Parameters
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