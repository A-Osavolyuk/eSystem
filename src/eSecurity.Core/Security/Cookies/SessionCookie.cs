namespace eSecurity.Core.Security.Cookies;

public class SessionCookie
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required Guid DeviceId { get; set; }
    public required DateTimeOffset IssuedAt { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
}