﻿using Features.Servers;
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

        public async Task<IActionResult> Index(GetServersByNameParameters parameters)
        {
            var servers = await _mediator.Send(new GetServersByNameQuery(parameters));
            return Json(new { results = servers.Select(x => new { Id = x.Url, Text = x.Url }), pagination = new { more = servers.PageNumber * servers.PageSize < servers.TotalItemCount } });
        }
    }
}