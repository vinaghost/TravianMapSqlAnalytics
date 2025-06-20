﻿using Features.Shared.Constraints;
using FluentValidation;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Features.Alliances.GetAlliancesById
{
    [Handler]
    public static partial class GetAlliancesByIdQuery
    {
        public sealed record Query(GetAlliancesByIdParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetAlliancesByIdQuery)}_{Parameters.Key()}");

        private static async ValueTask<IList<AllianceDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var ids = query.Parameters.Ids.Distinct().ToList();
            if (ids.Count == 0) return [];

            var queryable = context.Alliances.AsQueryable();

            if (ids.Count == 1)
            {
                queryable = queryable.Where(x => x.Id == ids[0]);
            }
            else
            {
                queryable = queryable.Where(x => ids.Contains(x.Id));
            }

            var alliances = await queryable
                .Select(x => new AllianceDto(
                    x.Id,
                    x.Name,
                    x.PlayerCount
                ))
                .ToListAsync(cancellationToken);

            return alliances;
        }
    }
}