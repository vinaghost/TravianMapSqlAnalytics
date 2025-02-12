using FluentValidation;

namespace WebAPI.Contracts.Requests
{
    public interface IIdsRequest
    {
        List<int> Ids { get; }
    }

    public class IdsRequestValidator : AbstractValidator<IIdsRequest>
    {
        public IdsRequestValidator()
        {
            RuleFor(x => x.Ids.Count).GreaterThan(0);
        }
    }
}