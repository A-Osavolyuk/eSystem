namespace eAccount.Blazor.Server.Infrastructure.Security.Authentication.SSO;

public class SessionCookie
{
    public required Guid SessionId { get; set; }
    public required Guid UserId { get; set; }
    public required Guid DeviceId { get; set; }
    public required string Nonce { get; set; } = string.Empty;
    public required DateTimeOffset IssuedAt { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
}