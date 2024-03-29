﻿using Features.GetServer;
using Core.Services;
using Features.Shared.Parameters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class ServersController(IMediator mediator, DataService dataService) : Controller
    {
        private readonly IMediator _mediator = mediator;
        private readonly DataService _dataService = dataService;

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
            var servers = await _mediator.Send(new Features.SearchServer.GetServerQuery(parameters));
            var serverList = servers
                .Select(x => new { Id = x.Text, x.Text });
            return Json(new { results = serverList, pagination = new { more = servers.PageNumber * servers.PageSize < servers.TotalItemCount } });
        }
    }
}