namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;

public sealed class LogoutContext
{
    public required string PostLogoutRedirectUri { get; set; }
    public required string Audience { get; set; }
    public required string Sid { get; set; }
    public string? State { get; set; }
}