using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.View;
using MapSqlAspNetCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace MapSqlAspNetCoreMVC.Controllers
{
    public class VillageFilterController : Controller
    {
        private readonly ILogger<VillageFilterController> _logger;
        private readonly IDataProvide _dataProvider;
        private readonly IConfiguration _configuration;

        public VillageFilterController(ILogger<VillageFilterController> logger, IConfiguration configuration, IDataProvide dataProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _dataProvider = dataProvider;
        }

        public async Task<IActionResult> Index(VillageFilterFormInput input)
        {
            input ??= new VillageFilterFormInput();
            var viewModel = await GetViewModel(input);
            return View(viewModel);
        }

        private async Task<VillageFilterViewModel> GetViewModel(VillageFilterFormInput input)
        {
            var villages = await _dataProvider.GetVillageData(input);
            var pageVillages = villages.ToPagedList(input.PageNumber, input.PageSize);
            var alliances = _dataProvider.GetAllianceSelectList();
            var tribes = _dataProvider.GetTribeSelectList();

            var viewModel = new VillageFilterViewModel
            {
                Server = _configuration["WorldUrl"],
                Input = input,
                VillageTotal = villages.Count,
                Villages = pageVillages,
                Alliances = alliances,
                Tribes = tribes
            };
            return viewModel;
        }
    }
}