namespace eSecurity.Server.Security.Cryptography.Tokens;

public interface ITokenFactoryProvider
{
    public ITokenFactory GetFactory(TokenType tokenType);
}