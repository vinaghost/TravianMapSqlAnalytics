using MapSqlQuery.Models.Input;
using MapSqlQuery.Models.View;
using MapSqlQuery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace MapSqlQuery.Controllers
{
    public class InactivePlayersController : Controller
    {
        private readonly ILogger<InactivePlayersController> _logger;
        private readonly IDataProvide _dataProvider;
        private readonly IConfiguration _configuration;

        public InactivePlayersController(ILogger<InactivePlayersController> logger, IDataProvide dataProvider, IConfiguration configuration)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var input = new InactiveFormInput();
            var viewModel = GetViewModel(input);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Form(InactiveFormInput input)
        {
            input ??= new InactiveFormInput();
            var viewModel = GetViewModel(input);
            return View("Index", viewModel);
        }

        [HttpGet]
        public IActionResult Paging(InactiveFormInput input)
        {
            input ??= new InactiveFormInput();
            var viewModel = GetViewModel(input);
            return View("Index", viewModel);
        }

        private InactivePlayerViewModel GetViewModel(InactiveFormInput input)
        {
            var players = _dataProvider.GetInactivePlayerData(input);
            var pagePlayers = players.ToPagedList(input.PageNumber, input.PageSize);
            var dates = _dataProvider.GetDateBefore(input.Days);

            var viewModel = new InactivePlayerViewModel
            {
                Server = _configuration["WorldUrl"],
                Input = input,
                PlayerTotal = players.Count,
                Dates = dates,
                Players = pagePlayers
            };
            return viewModel;
        }
    }
}