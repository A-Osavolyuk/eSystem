using eSystem.Core.Security.Authentication.OpenIdConnect.Client;

namespace eSecurity.Core.Security.Authentication.OpenIdConnect.Client;

public class ClientInfo
{
    public string ClientName { get; set; } = string.Empty;
    public ClientType ClientType { get; set; }
    public List<string> RequiredScopes { get; set; } = [];
}