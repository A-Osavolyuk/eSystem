using eSecurity.Security.Authentication.JWT.Payloads;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Tokens;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.JWT.Factories;

public class AccessTokenFactory(
    IOptions<JwtOptions> options,
    IJwtSigner jwtSigner) : ITokenFactory
{
    private readonly IJwtSigner jwtSigner = jwtSigner;
    private readonly JwtOptions options = options.Value;

    public string Create(TokenPayload payload)
    {
        if (payload is not AccessTokenPayload accessPayload)
            throw new NotImplementedException("Payload type must be 'AccessTokenPayload'");

        var claimBuilder = ClaimBuilder.Create()
            .WithTokenId(Guid.CreateVersion7().ToString())
            .WithSubject(accessPayload.Subject)
            .WithIssuer(options.Issuer)
            .WithAudience(accessPayload.Audience)
            .WithIssuedTime(accessPayload.IssuedAt)
            .WithExpirationTime(accessPayload.ExpiresAt)
            .WithScope(accessPayload.Scopes);

        if (!string.IsNullOrEmpty(accessPayload.Nonce))
            claimBuilder.WithNonce(accessPayload.Nonce);
        
        return jwtSigner.Sign(claimBuilder.Build(), options.Secret, SecurityAlgorithms.HmacSha256);
    }
}