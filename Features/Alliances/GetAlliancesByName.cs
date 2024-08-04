using Features.Shared.Dtos;
using Features.Shared.Parameters;
using Features.Shared.Query;
using Features.Shared.Validators;
using FluentValidation;
using MediatR;
using System.Text;
using X.PagedList;

namespace Features.Alliances
{
    public record GetAlliancesByNameParameters : IPaginationParameters, ISearchTermParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? SearchTerm { get; init; }
    }

    public static class GetAlliancesByNameParametersExtension
    {
        public static string Key(this GetAlliancesByNameParameters parameters)
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

    public class GetAllianceByNameParametersValidator : AbstractValidator<GetAlliancesByNameParameters>
    {
        public GetAllianceByNameParametersValidator()
        {
            Include(new PaginationParametersValidator());
        }
    }

    public record GetAlliancesByNameQuery(GetAlliancesByNameParameters Parameters) : ICachedQuery<IPagedList<AllianceDto>>
    {
        public string CacheKey => $"{nameof(GetAlliancesByNameQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetAlliancesByNameQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetAlliancesByNameQuery, IPagedList<AllianceDto>>
    {
        private readonly VillageDbContext _dbContext = dbContext;

        public async Task<IPagedList<AllianceDto>> Handle(GetAlliancesByNameQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;
            var query = _dbContext.Alliances
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                query = query
                    .Where(x => x.Name.StartsWith(parameters.SearchTerm));
            }

            query = query
                .Where(x => x.Players.Count > 0);

            var data = await query
                .OrderBy(x => x.Name)
                .Select(x => new AllianceDto(
                        x.Id,
                        string.IsNullOrWhiteSpace(x.Name) ? "No alliance" : x.Name,
                        x.PlayerCount
                ))
                .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);

            return data;
        }
    }
}