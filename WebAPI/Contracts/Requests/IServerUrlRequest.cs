using FluentValidation;

namespace WebAPI.Contracts.Requests
{
    public interface IServerUrlRequest
    {
        string ServerUrl { get; }
    }

    public class ServerUrlRequestValidator : AbstractValidator<IServerUrlRequest>
    {
        public ServerUrlRequestValidator()
        {
            RuleFor(x => x.ServerUrl)
                .NotEmpty();
        }
    }
}