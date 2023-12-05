using Core;
using MediatR;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetDateBeforeQuery(int Days) : IRequest<List<DateTime>>;

    public class GetDateBeforeQueryHandler : IRequestHandler<GetDateBeforeQuery, List<DateTime>>
    {
        private readonly ServerDbContext _context;

        public GetDateBeforeQueryHandler(ServerDbContext context)
        {
            _context = context;
        }

        public async Task<List<DateTime>> Handle(GetDateBeforeQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return _context.GetDateBefore(request.Days);
        }
    }
}