using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Players.GetPlayers
{
    public class GetPlayersParametersValidator : AbstractValidator<GetPlayersParameters>
    {
        public GetPlayersParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new PlayerFilterParametersValidator());
        }
    }
}