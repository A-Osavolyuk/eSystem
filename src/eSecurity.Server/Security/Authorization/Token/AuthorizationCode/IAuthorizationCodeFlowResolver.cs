using eSecurity.Server.Security.Authorization.Protocol;

namespace eSecurity.Server.Security.Authorization.Token.AuthorizationCode;

public interface IAuthorizationCodeFlowResolver
{
    public IAuthorizationCodeFlow Resolve(AuthorizationProtocol protocol);
}