using MediatR;

namespace Features.Shared.Query
{
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }
}