using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Populations
{
    public class PopulationParametersValidator : AbstractValidator<PopulationParameters>
    {
        public PopulationParametersValidator()
        {
            Include(new PopulationFilterParametersValidator());
        }
    }
}