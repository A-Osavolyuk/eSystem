namespace eSecurity.Security.Authorization.OAuth;

public class ProviderOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public bool SaveTokens { get; set; }
    public string CallbackPath { get; set; } = string.Empty;
}