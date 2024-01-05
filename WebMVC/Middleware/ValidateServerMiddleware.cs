﻿using Core.Queries;
using Core.Services;
using MediatR;

namespace WebMVC.Middleware
{
    public class ValidateServerMiddleware(IMediator mediator, DataService dataService) : IMiddleware
    {
        private readonly IMediator _mediator = mediator;
        private readonly DataService _dataService = dataService;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var server = context.Request.Cookies["server"];
            if (string.IsNullOrEmpty(server))
            {
                server = await _mediator.Send(new GetMostPlayerServerQuery());
            }

            _dataService.Server = server;

            context.Response
                   .Cookies
                   .Append("server", server);

            await next(context);
        }
    }
}