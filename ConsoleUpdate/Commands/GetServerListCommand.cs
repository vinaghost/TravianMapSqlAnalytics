﻿using ConsoleUpdate.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ConsoleUpdate.Commands
{
    public record GetServerListCommand : IRequest<List<ServerRaw>>;

    public class GetServerListCommandHandler : IRequestHandler<GetServerListCommand, List<ServerRaw>>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GetServerListCommand> _logger;
        private const string _url = "https://travian4bot.com/api/server?searchType=Servers&limit=500";

        public GetServerListCommandHandler(HttpClient httpClient, ILogger<GetServerListCommand> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ServerRaw>> Handle(GetServerListCommand request, CancellationToken cancellationToken)
        {
            var responseMessage = await _httpClient.GetFromJsonAsync<ResponseMessage>(_url, cancellationToken: cancellationToken);
            var servers = responseMessage?.Rows ?? [];
            _logger.LogInformation("Found {count} servers", servers.Count);
            return servers;
        }

        private class ResponseMessage
        {
            public int Count { get; set; }
            public List<ServerRaw>? Rows { get; set; }
        }
    }
}