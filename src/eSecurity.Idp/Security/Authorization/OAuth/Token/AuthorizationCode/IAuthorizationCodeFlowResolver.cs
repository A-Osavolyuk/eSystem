using eSecurity.Idp.Security.Authorization.OAuth.Protocol;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.AuthorizationCode;

public interface IAuthorizationCodeFlowResolver
{
    public IAuthorizationCodeFlow Resolve(AuthorizationProtocol protocol);
}