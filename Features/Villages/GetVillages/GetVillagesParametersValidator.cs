using Features.Players;
using Features.Shared.Parameters;
using FluentValidation;

namespace Features.Villages.GetVillages
{
    public class GetVillagesParametersValidator : AbstractValidator<GetVillagesParameters>
    {
        public GetVillagesParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new DistanceFilterParametersValidator());
            Include(new PlayerFilterParametersValidator());
            Include(new VillageFilterParametersValidator());
        }
    }
}