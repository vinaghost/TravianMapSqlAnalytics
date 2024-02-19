using Core.Features.Shared.Dtos;
using MediatR;
using X.PagedList;

namespace Core.Features.GetNeighbors
{
    public class GetNeighborsQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetNeighborsQuery, IPagedList<VillageDataDto>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public Task<IPagedList<VillageDataDto>> Handle(GetNeighborsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}