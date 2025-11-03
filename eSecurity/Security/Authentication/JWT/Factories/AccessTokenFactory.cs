using System.IdentityModel.Tokens.Jwt;
using eSecurity.Security.Authentication.JWT.Payloads;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Tokens;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.JWT.Factories;

public class AccessTokenFactory(IOptions<JwtOptions> options) : ITokenFactory
{
    private readonly JwtOptions options = options.Value;

    public string Create(TokenPayload payload)
    {
        if (payload is not AccessTokenPayload accessPayload)
            throw new NotImplementedException("Payload type must be 'AccessTokenPayload'");

        var claimBuilder = ClaimBuilder.Create()
            .WithTokenId(Guid.CreateVersion7().ToString())
            .WithSubject(accessPayload.Subject)
            .WithAudience(accessPayload.Audience)
            .WithIssuedTime(accessPayload.IssuedAt)
            .WithExpirationTime(accessPayload.ExpiresAt)
            .WithScope(accessPayload.Scopes);

        if (!string.IsNullOrEmpty(accessPayload.Nonce))
            claimBuilder.WithNonce(accessPayload.Nonce);

        const string algorithm = SecurityAlgorithms.HmacSha256;

        var key = Encoding.UTF8.GetBytes(options.Secret);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm);

        var securityToken = new JwtSecurityToken(
            issuer: options.Issuer,
            claims: claimBuilder.Build(),
            signingCredentials: signingCredentials);

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(securityToken);
    }
}