﻿using Features.Shared.Constraints;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Features.Villages.GetVillagesByPlayerId
{
    [Handler]
    public static partial class GetVillagesByPlayerIdQuery
    {
        public sealed record Query(int PlayerId) : DefaultCachedQuery($"{nameof(GetVillagesByPlayerIdQuery)}_{PlayerId}");

        private static async ValueTask<IList<VillageDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var playerId = query.PlayerId;
            var villages = await context.Villages
                .Where(x => x.PlayerId == playerId)
                .Select(x => new VillageDto(
                    x.MapId,
                    x.Id,
                    x.Name,
                    x.X,
                    x.Y,
                    (Tribe)x.Tribe,
                    x.Population,
                    x.IsCapital
                ))
                .ToListAsync(cancellationToken);
            return villages;
        }
    }
}