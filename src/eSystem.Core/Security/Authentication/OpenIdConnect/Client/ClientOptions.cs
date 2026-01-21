namespace eSystem.Core.Security.Authentication.OpenIdConnect.Client;

public class ClientOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string ClientAudience { get; set; } = string.Empty;
    public string CallbackUri { get; set; } = string.Empty;
    public string PostLogoutRedirectUri { get; set; } = string.Empty;
    public string[] SupportedScopes { get; set; } = [];
    public string[] SupportedPrompts { get; set; } = [];
};