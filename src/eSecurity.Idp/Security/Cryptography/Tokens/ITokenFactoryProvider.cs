using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Cryptography.Tokens;

public interface ITokenFactoryProvider
{
    public ITokenFactory GetFactory(TokenType tokenType);
}