using System.Security.Claims;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Tokens;

namespace eSecurity.Security.Authentication.JWT.Factories;

public class IdTokenFactory(
    IOptions<JwtOptions> options,
    IJwtSigner jwtSigner) : ITokenFactory
{
    private readonly IJwtSigner jwtSigner = jwtSigner;
    private readonly JwtOptions options = options.Value;

    public string Create(IEnumerable<Claim> claims) 
        => jwtSigner.Sign(claims, options.Secret, SecurityAlgorithms.HmacSha256);
}