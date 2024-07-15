using MediatR;

namespace Features.Shared.Query
{
    public interface IQuery<out TResponse> : IRequest<TResponse>;
}