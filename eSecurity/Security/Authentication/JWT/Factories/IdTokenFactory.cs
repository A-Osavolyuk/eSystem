using System.IdentityModel.Tokens.Jwt;
using eSecurity.Security.Authentication.JWT.Enrichers;
using eSecurity.Security.Authentication.JWT.Payloads;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Tokens;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.JWT.Factories;

public class IdTokenFactory(
    IOptions<JwtOptions> options,
    IEnumerable<IClaimEnricher> enrichers,
    IJwtSigner jwtSigner) : ITokenFactory
{
    private readonly IEnumerable<IClaimEnricher> enrichers = enrichers;
    private readonly IJwtSigner jwtSigner = jwtSigner;
    private readonly JwtOptions options = options.Value;

    public string Create(TokenPayload payload)
    {
        if (payload is not IdTokenPayload idPayload)
            throw new NotImplementedException("Payload type must be 'IdTokenPayload'");

        var claimBuilder = ClaimBuilder.Create()
            .WithTokenId(Guid.CreateVersion7().ToString())
            .WithSubject(idPayload.Subject)
            .WithAudience(idPayload.Audience)
            .WithIssuer(options.Issuer)
            .WithIssuedTime(idPayload.IssuedAt)
            .WithExpirationTime(idPayload.ExpiresAt);

        if (!string.IsNullOrEmpty(idPayload.Nonce))
            claimBuilder.WithNonce(idPayload.Nonce);

        foreach (var enricher in enrichers)
            enricher.Enrich(claimBuilder, idPayload);

        return jwtSigner.Sign(claimBuilder.Build(), options.Secret, SecurityAlgorithms.HmacSha256);
    }
}