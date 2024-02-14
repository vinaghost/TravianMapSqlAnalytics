﻿using Core.Features.GetServer;
using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using Core.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class ServersController(IMediator mediator, DataService dataService) : Controller
    {
        private readonly IMediator _mediator = mediator;
        private readonly DataService _dataService = dataService;

        public IActionResult Index(ServerParameters parameters)
        {
            return View(parameters);
        }

        public async Task<IActionResult> Change(string server)
        {
            var isValid = await _mediator.Send(new ValidateServerUrlQuery(server));
            if (isValid)
            {
                _dataService.Server = server;

                var options = new CookieOptions()
                {
                    Expires = new DateTimeOffset(DateTime.Now.AddYears(1)),
                    SameSite = SameSiteMode.Lax
                };

                Response.Cookies.Append("TMSA_server", server, options);
                return Ok();
            }
            return NotFound();
        }

        public async Task<IActionResult> ServerList(SearchParameters parameters)
        {
            var servers = await _mediator.Send<IList<SearchResult>>(new Core.Features.SearchServer.GetServerQuery(parameters));
            var serverList = servers
                .Select(x => new { Id = x.Text, Text = x.Text })
                .Take(20)
                .ToList();
            return Json(new { items = serverList });
        }
    }
}