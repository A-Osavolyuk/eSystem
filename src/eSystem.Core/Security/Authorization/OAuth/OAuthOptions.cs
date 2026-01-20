namespace eSystem.Core.Security.Authorization.OAuth;

public class OAuthOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public bool SaveTokens { get; set; }
    public string CallbackPath { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
    public string ErrorPath { get; set; } = string.Empty;
}