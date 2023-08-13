﻿using MapSqlQuery.Models;
using MapSqlQuery.Services.Interfaces;
using MapSqlQuery.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MapSqlQuery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataProvide _dataProvider;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IDataProvide dataProvider, IConfiguration configuration)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.NewestDateTime = DateTime.Today.ToString("yyyy-MM-dd");
            ViewData["Server"] = _configuration["WorldUrl"];
            return View();
        }

        public IActionResult VillageFilter(VillageFilterViewModel viewModel)
        {
            ViewData["Server"] = _configuration["WorldUrl"];

            viewModel.Villages = _dataProvider.GetVillageData(viewModel.FormInput);
            viewModel.Alliances = _dataProvider.GetAllianceSelectList();
            viewModel.Tribes = _dataProvider.GetTribeSelectList();
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}