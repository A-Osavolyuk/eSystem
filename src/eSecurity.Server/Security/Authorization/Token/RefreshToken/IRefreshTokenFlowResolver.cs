using eSecurity.Server.Security.Authorization.Protocol;

namespace eSecurity.Server.Security.Authorization.Token.RefreshToken;

public interface IRefreshTokenFlowResolver
{
    public IRefreshTokenFlow Resolve(AuthorizationProtocol protocol);
}