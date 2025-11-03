using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Tokens;

namespace eSecurity.Security.Authentication.JWT;

public interface ITokenFactoryResolver
{
    public ITokenFactory Create(JwtTokenType type);
}