﻿using Features.Players;
using Features.Populations;
using Features.Villages;

namespace WebMVC.Models.ViewModel.Players
{
    public class IndexViewModel
    {
        public PlayerDto? Player { get; set; }
        public IList<DetailVillageDto> Villages { get; set; } = [];
        public Dictionary<int, List<PopulationDto>> Population { get; set; } = [];
    }
}