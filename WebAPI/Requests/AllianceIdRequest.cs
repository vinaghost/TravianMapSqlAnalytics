namespace WebAPI.Requests
{
    public record AllianceIdRequest(string ServerUrl, int AllianceId) : IServerUrlRequest;
}