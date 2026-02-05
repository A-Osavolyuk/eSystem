using eSecurity.Server.Security.Authorization.OAuth.Protocol;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public interface IAuthorizationCodeFlowResolver
{
    public IAuthorizationCodeFlow Resolve(AuthorizationProtocol protocol);
}