using Features.Shared.Parameters;
using Features.Shared.Query;
using Features.Shared.Validators;
using FluentValidation;
using MediatR;
using System.Text;
using X.PagedList;

namespace Features.Players
{
    public record GetPlayerByNameParameters : IPaginationParameters, ISearchTermParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? Name { get; init; }
    }

    public static class GetPlayerByNameParametersExtension
    {
        public static string Key(this GetPlayerByNameParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            sb.Append(parameters.PageNumber);
            sb.Append(SEPARATOR);
            sb.Append(parameters.PageSize);
            sb.Append(SEPARATOR);
            sb.Append(parameters.Name);

            return sb.ToString();
        }
    }

    public class GetPlayerByNameParametersValidator : AbstractValidator<GetPlayerByNameParameters>
    {
        public GetPlayerByNameParametersValidator()
        {
            Include(new PaginationParametersValidator());
        }
    }

    public record GetPlayerByNameQuery(GetPlayerByNameParameters Parameters) : ICachedQuery<IPagedList<PlayerDto>>
    {
        public string CacheKey => $"{nameof(GetPlayerByNameQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetPlayerByNameQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetPlayerByNameQuery, IPagedList<PlayerDto>>
    {
        private readonly VillageDbContext _dbContext = dbContext;

        public async Task<IPagedList<PlayerDto>> Handle(GetPlayerByNameQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;
            var query = _dbContext.Players
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.Name))
            {
                query = query
                    .Where(x => x.Name.StartsWith(parameters.Name));
            }

            var data = await query
                .OrderBy(x => x.Name)
                .Select(x => new PlayerDto(
                        x.Id,
                        x.Name,
                        x.VillageCount,
                        x.Population
                ))
                .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);

            return data;
        }
    }
}