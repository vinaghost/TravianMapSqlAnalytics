using ConsoleUpdate.Models;
using HtmlAgilityPack;
using MediatR;

namespace ConsoleUpdate.Commands
{
    public record GetServerListCommand : IRequest<List<ServerRaw>>;

    public class GetServerListCommandHandler(HttpClient httpClient) : IRequestHandler<GetServerListCommand, List<ServerRaw>>
    {
        private readonly HttpClient _httpClient = httpClient;
        private const string _url = "https://travcotools.com/en/inactive-search/";

        public async Task<List<ServerRaw>> Handle(GetServerListCommand request, CancellationToken cancellationToken)
        {
            var html = await _httpClient.GetStreamAsync(_url, cancellationToken);
            var doc = new HtmlDocument();
            doc.Load(html);

            var select = doc.DocumentNode.Descendants("select").FirstOrDefault(x => x.GetAttributeValue("name", "") == "travian_server");
            if (select is null) return [];

            var options = select.Descendants("option")
                .Where(x => !string.IsNullOrEmpty(x.GetAttributeValue("value", "")))
                .Select(x => new ServerRaw()
                {
                    Id = x.GetAttributeValue("value", 0),
                    Url = x.InnerText,
                });

            return options.ToList();
        }
    }
}