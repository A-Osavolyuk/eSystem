using eSecurity.Idp.Security.Authorization.OAuth.Protocol;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.RefreshToken;

public interface IRefreshTokenFlowResolver
{
    public IRefreshTokenFlow Resolve(AuthorizationProtocol protocol);
}