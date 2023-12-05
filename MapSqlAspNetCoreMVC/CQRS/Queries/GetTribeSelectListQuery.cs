using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetTribeSelectListQuery : IRequest<List<SelectListItem>>;

    public class GetTribeSelectListQueryHandler : IRequestHandler<GetTribeSelectListQuery, List<SelectListItem>>
    {
        private static readonly List<SelectListItem> _tribeNamesList = new()
        {
            new SelectListItem {Value = "0", Text = "All"},
            new SelectListItem {Value = "1", Text = "Romans"},
            new SelectListItem {Value = "2", Text = "Teutons"},
            new SelectListItem {Value = "3", Text = "Gauls"},
            new SelectListItem {Value = "4", Text = "Nature"},
            new SelectListItem {Value = "5", Text = "Natars"},
            new SelectListItem {Value = "6", Text = "Egyptians"},
            new SelectListItem {Value = "7", Text = "Huns"},
            new SelectListItem {Value = "8", Text = "Spartans"},
        };

        public async Task<List<SelectListItem>> Handle(GetTribeSelectListQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return _tribeNamesList;
        }
    }
}