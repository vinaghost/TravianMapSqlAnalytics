using Core;
using MediatR;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetNewestDayQuery : IRequest<string>;

    public class GetNewestDayQueryHandler : IRequestHandler<GetNewestDayQuery, string>
    {
        private readonly ServerDbContext _context;

        public GetNewestDayQueryHandler(ServerDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(GetNewestDayQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return _context.GetNewestDay().ToString("yyyy-MM-dd");
        }
    }
}