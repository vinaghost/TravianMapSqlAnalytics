using FluentValidation;

namespace WebAPI.Contracts.Requests
{
    public interface IIdRequest
    {
        int Id { get; }
    }

    public class IdRequestValidator : AbstractValidator<IIdRequest>
    {
        public IdRequestValidator()
        {
            RuleFor(x => x.Id).GreaterThanOrEqualTo(0);
        }
    }
}