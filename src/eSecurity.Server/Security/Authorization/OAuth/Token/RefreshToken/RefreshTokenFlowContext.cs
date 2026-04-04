using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;

public sealed class RefreshTokenFlowContext
{
    public required string RefreshToken { get; set; }
    public required GrantType GrantType { get; set; }
    public required string ClientId { get; set; }
}