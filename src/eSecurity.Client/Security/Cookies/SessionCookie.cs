namespace eSecurity.Client.Security.Cookies;

public class SessionCookie
{
    public required Guid Id { get; set; }
    public required DateTimeOffset IssuedAt { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
}