namespace eSecurity.Client.Security.Authentication.Oidc.Clients;

public class ClientOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CallbackUri { get; set; } = string.Empty;
    public string PostLogoutRedirectUri { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = [];
};