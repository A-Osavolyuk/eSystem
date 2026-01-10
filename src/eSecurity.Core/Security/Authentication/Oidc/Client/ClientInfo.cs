namespace eSecurity.Core.Security.Authentication.Oidc.Client;

public class ClientInfo
{
    public string ClientName { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
    public string? ClientDescription { get; set; }
    public string? LogoUri { get; set; }
    public string? ClientUri { get; set; }
    public List<string> RequiredScopes { get; set; } = [];
}