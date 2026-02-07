using eSecurity.Server.Security.Authorization.Constants;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Transformation;

public interface ITokenTransformationHandlerResolver
{
    public ITokenTransformationHandler Resolve(TokenKind kind);
}