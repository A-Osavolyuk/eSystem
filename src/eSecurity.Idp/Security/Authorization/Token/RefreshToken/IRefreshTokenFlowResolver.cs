namespace eSecurity.Idp.Security.Authorization.Token.RefreshToken;

public interface IRefreshTokenFlowResolver
{
    public IRefreshTokenFlow Resolve(AuthorizationProtocol protocol);
}