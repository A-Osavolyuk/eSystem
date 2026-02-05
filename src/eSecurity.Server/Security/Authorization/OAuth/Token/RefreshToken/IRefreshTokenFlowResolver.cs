using eSecurity.Server.Security.Authorization.OAuth.Protocol;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;

public interface IRefreshTokenFlowResolver
{
    public IRefreshTokenFlow Resolve(AuthorizationProtocol protocol);
}