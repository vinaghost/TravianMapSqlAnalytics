using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Players.GetPlayersByName
{
    public class GetPlayersByNameParametersValidator : AbstractValidator<GetPlayersByNameParameters>
    {
        public GetPlayersByNameParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new SearchTermParametersValidator());
        }
    }
}