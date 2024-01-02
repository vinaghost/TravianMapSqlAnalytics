using MediatR;

namespace Core.Queries
{
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }
}