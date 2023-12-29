using MediatR;

namespace WebAPI.Queries
{
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }
}