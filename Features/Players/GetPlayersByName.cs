using Features.Shared.Parameters;
using Features.Shared.Query;
using Features.Shared.Validators;
using FluentValidation;
using MediatR;
using System.Text;
using X.PagedList;

namespace Features.Players
{
    public record GetPlayersByNameParameters : IPaginationParameters, ISearchTermParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? SearchTerm { get; init; }
    }

    public static class GetPlayersByNameParametersExtension
    {
        public static string Key(this GetPlayersByNameParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            sb.Append(parameters.PageNumber);
            sb.Append(SEPARATOR);
            sb.Append(parameters.PageSize);
            sb.Append(SEPARATOR);
            sb.Append(parameters.SearchTerm);

            return sb.ToString();
        }
    }

    public class GetPlayersByNameParametersValidator : AbstractValidator<GetPlayersByNameParameters>
    {
        public GetPlayersByNameParametersValidator()
        {
            Include(new PaginationParametersValidator());
        }
    }

    public record GetPlayersByNameQuery(GetPlayersByNameParameters Parameters) : ICachedQuery<IPagedList<PlayerDto>>
    {
        public string CacheKey => $"{nameof(GetPlayersByNameQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetPlayersByNameQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetPlayersByNameQuery, IPagedList<PlayerDto>>
    {
        private readonly VillageDbContext _dbContext = dbContext;

        public async Task<IPagedList<PlayerDto>> Handle(GetPlayersByNameQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;
            var query = _dbContext.Players
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                query = query
                    .Where(x => x.Name.StartsWith(parameters.SearchTerm));
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