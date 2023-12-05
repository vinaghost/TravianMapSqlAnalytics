using Core;
using MediatR;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetNewestDayQuery : IRequest<DateTime>;

    public class GetNewestDayQueryHandler : IRequestHandler<GetNewestDayQuery, DateTime>
    {
        private readonly ServerDbContext _context;

        public GetNewestDayQueryHandler(ServerDbContext context)
        {
            _context = context;
        }

        public async Task<DateTime> Handle(GetNewestDayQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return _context.GetNewestDay();
        }
    }
}