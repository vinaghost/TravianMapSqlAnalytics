﻿using MapSqlQuery.Models;
using MapSqlQuery.Models.Form;
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
            ViewBag.NewestDateTime = _dataProvider.NewestDateStr;
            return View();
        }

        public async Task<IActionResult> InactivePlayers(InactivePlayerViewModel viewModel = null!)
        {
            ViewData["Server"] = _configuration["WorldUrl"];
            var days = viewModel.FormInput?.Days ?? 3;
            ViewData["Days"] = days;

            var players = await _dataProvider.GetPlayerData(_dataProvider.NewestDate, days);
            viewModel.Players = players;
            viewModel.FormInput ??= new InactiveFormInput();
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}