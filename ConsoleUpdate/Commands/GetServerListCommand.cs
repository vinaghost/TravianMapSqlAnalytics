using ConsoleUpdate.Models;
using MediatR;
using System.Net.Http.Json;

namespace ConsoleUpdate.Commands
{
    public record GetServerListCommand : IRequest<List<ServerRaw>>;

    public class GetServerListCommandHandler(HttpClient httpClient) : IRequestHandler<GetServerListCommand, List<ServerRaw>>
    {
        private readonly HttpClient _httpClient = httpClient;
        private const string _url = "https://travian4bot.com/api/server?searchType=Servers&limit=500";

        public async Task<List<ServerRaw>> Handle(GetServerListCommand request, CancellationToken cancellationToken)
        {
            var responseMessage = await _httpClient.GetFromJsonAsync<ResponseMessage>(_url, cancellationToken: cancellationToken);
            var servers = responseMessage?.Rows ?? [];
            return servers;
        }

        private class ResponseMessage
        {
            public int Count { get; set; }
            public List<ServerRaw>? Rows { get; set; }
        }
    }
}