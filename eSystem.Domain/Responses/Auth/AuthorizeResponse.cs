namespace eSystem.Domain.Responses.Auth;

public class AuthorizeResponse
{
    public required Guid UserId { get; set; }
    public required Guid DeviceId { get; set; }
    public required Guid SessionId { get; set; }
    public required string State { get; set; }
    public required string Nonce { get; set; }
    public required string ClientId { get; set; }
    public required string RedirectUri { get; set; }
}