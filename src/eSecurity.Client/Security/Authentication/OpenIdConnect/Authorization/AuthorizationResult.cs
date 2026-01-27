namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;

public sealed class AuthorizationResult
{
    private AuthorizationResult() { }
    
    public string? RedirectUri { get; set; }
    
    public static AuthorizationResult Next() => new();
    public static AuthorizationResult Redirect(string redirectUri) => new() { RedirectUri = redirectUri };
}