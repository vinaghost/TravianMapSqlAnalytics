using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Alliances.GetAlliancesByName
{
    public class GetAllianceByNameParametersValidator : AbstractValidator<GetAlliancesByNameParameters>
    {
        public GetAllianceByNameParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new SearchTermParametersValidator());
        }
    }
}