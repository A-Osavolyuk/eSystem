namespace eSecurity.Idp.Security.Authorization.Token.AuthorizationCode;

public interface IAuthorizationCodeFlowResolver
{
    public IAuthorizationCodeFlow Resolve(AuthorizationProtocol protocol);
}