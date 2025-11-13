using eSecurity.Core.Security.Authentication.Oidc.Logout;

namespace eSecurity.Core.Common.Requests;

public class LogoutRequest
{
    public LogoutType Type { get; set; }
    public string? ClientId { get; set; }
    public string? IdTokenHint { get; set; }
    public string? PostLogoutRedirectUri { get; set; }
    public string? State { get; set; }
}