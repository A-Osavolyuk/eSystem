namespace eSecurity.Server.Security.Authorization.Token.RefreshToken;

public sealed class RefreshTokenFlowContext
{
    public required string RefreshToken { get; set; }
    public required string GrantType { get; set; }
    public required string ClientId { get; set; }
}