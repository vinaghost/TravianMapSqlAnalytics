namespace WebAPI.Contracts.Requests
{
    public record IdRequest(string ServerUrl, int Id) : IServerUrlRequest;
}