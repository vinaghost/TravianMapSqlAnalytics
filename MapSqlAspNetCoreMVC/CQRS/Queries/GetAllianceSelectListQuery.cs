using Core;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetAllianceSelectListQuery : IRequest<List<SelectListItem>>;

    public class GetAllianceSelectListQueryHandler : IRequestHandler<GetAllianceSelectListQuery, List<SelectListItem>>
    {
        private readonly ServerDbContext _context;

        public GetAllianceSelectListQueryHandler(ServerDbContext context)
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> Handle(GetAllianceSelectListQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var alliances = new List<SelectListItem>
            {
                new() { Value = "-1", Text = "All" }
            };

            var query = _context.Alliances
                .Include(x => x.Players)
                .OrderByDescending(x => x.Players.Count)
                .Select(x => new SelectListItem
                {
                    Value = $"{x.AllianceId}",
                    Text = x.Name,
                })
                .AsEnumerable();

            alliances.AddRange(query);
            return alliances;
        }
    }
}