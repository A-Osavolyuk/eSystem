using eSecurity.Server.Security.Authorization.Protocol;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.AuthorizationCode;

public interface IAuthorizationCodeFlowResolver
{
    public IAuthorizationCodeFlow Resolve(AuthorizationProtocol protocol);
}