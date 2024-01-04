using MediatR;

namespace ConsoleUpdate.Commands
{
    public record ValidateServerCommand(string ServerUrl) : IRequest<bool>;

    public class ValidateServerCommandHandler(HttpClient httpClient) : IRequestHandler<ValidateServerCommand, bool>
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<bool> Handle(ValidateServerCommand request, CancellationToken cancellationToken)
        {
            var url = $"https://{request.ServerUrl}";
            try
            {
                //var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url), cancellationToken);
                var response = await _httpClient.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode) return false;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}