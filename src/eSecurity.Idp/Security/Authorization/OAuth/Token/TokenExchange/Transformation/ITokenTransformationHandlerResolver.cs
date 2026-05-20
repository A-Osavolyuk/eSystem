using eSecurity.Idp.Security.Authorization.Constants;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.TokenExchange.Transformation;

public interface ITokenTransformationHandlerResolver
{
    public ITokenTransformationHandler Resolve(TokenKind kind);
}