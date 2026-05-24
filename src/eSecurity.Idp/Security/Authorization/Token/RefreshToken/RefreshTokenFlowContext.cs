using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Authorization.Token.RefreshToken;

public sealed class RefreshTokenFlowContext
{
    public required string RefreshToken { get; set; }
    public required GrantType GrantType { get; set; }
    public required string ClientId { get; set; }
}