namespace WebAPI.Requests
{
    public record IdRequest(string ServerUrl, int Id) : IServerUrlRequest;
}