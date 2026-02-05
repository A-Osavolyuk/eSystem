namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public sealed class AuthorizationCodeFlowContext
{
    public required string ClientId { get; set; }
    public required string GrantType { get; set; }
    public required string RedirectUri { get; set; }
    public string? CodeVerifier { get; set; }
}
