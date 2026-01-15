namespace eCinema.Server.Security.Cookies;

public class SessionCookie
{
    public Guid Id { get; set; }
    public required DateTimeOffset IssuedAt { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
}