using Core.Features.Shared.Parameters;
using FluentValidation;

namespace Core.Features.Shared.Validators
{
    public class PlayerFilterParametersValidator : AbstractValidator<IPlayerFilterParameters>
    {
        public PlayerFilterParametersValidator()
        {
        }
    }
}