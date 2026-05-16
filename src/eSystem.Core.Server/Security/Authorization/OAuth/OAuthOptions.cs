namespace eSystem.Core.Server.Security.Authorization.OAuth;

public class OAuthOptions
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required bool SaveTokens { get; set; }

    public string ResponseType { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public bool UsePkce { get; set; }
    public bool MapInboundClaims { get; set; }
    public bool GetClaimsFromUserInfoEndpoint { get; set; }
    public string CallbackPath { get; set; } = string.Empty;
    public string SignedOutCallbackPath { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
    public string ErrorPath { get; set; } = string.Empty;
    public string SignedOutRedirectUri { get; set; } = string.Empty;
}