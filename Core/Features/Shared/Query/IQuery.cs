using MediatR;

namespace Core.Features.Shared.Query
{
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }
}