using eSecurity.Idp.Security.Authorization.Constants;

namespace eSecurity.Idp.Security.Authorization.Token.TokenExchange.Transformation;

public interface ITokenTransformationHandlerResolver
{
    public ITokenTransformationHandler Resolve(TokenKind kind);
}