using FluentValidation;

namespace Features.Alliances.GetAlliancesById
{
    public class GetAlliancesByIdParametersValidator : AbstractValidator<GetAlliancesByIdParameters>
    {
        public GetAlliancesByIdParametersValidator()
        {
            RuleFor(x => x.Ids).NotEmpty();
        }
    }
}