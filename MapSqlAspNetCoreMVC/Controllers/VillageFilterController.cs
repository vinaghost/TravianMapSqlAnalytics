using MapSqlQuery.Models.Input;
using MapSqlQuery.Models.View;
using MapSqlQuery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace MapSqlQuery.Controllers
{
    public class VillageFilterController : Controller
    {
        private readonly ILogger<VillageFilterController> _logger;
        private readonly IDataProvide _dataProvider;
        private readonly IConfiguration _configuration;

        public VillageFilterController(ILogger<VillageFilterController> logger, IDataProvide dataProvider, IConfiguration configuration)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _configuration = configuration;
        }

        public IActionResult Index(VillageFilterFormInput input)
        {
            input ??= new VillageFilterFormInput();
            var viewModel = GetViewModel(input);
            return View(viewModel);
        }

        private VillageFilterViewModel GetViewModel(VillageFilterFormInput input)
        {
            var villages = _dataProvider.GetVillageData(input);
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