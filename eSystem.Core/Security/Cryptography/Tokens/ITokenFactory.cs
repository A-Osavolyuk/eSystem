namespace eSystem.Core.Security.Cryptography.Tokens;

public interface ITokenFactory
{
    public string Create(TokenPayload payload);
}