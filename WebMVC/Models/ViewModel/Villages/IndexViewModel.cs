﻿using Features.Shared.Dtos;
using Features.Villages.Shared;
using X.PagedList;

namespace WebMVC.Models.ViewModel.Villages
{
    public class IndexViewModel
    {
        public VillagesParameters Parameters { get; init; } = new();
        public IPagedList<Features.Villages.ByDistance.VillageDto>? Villages { get; init; }
        public Dictionary<int, List<PopulationDto>> Population { get; init; } = [];
    }
}