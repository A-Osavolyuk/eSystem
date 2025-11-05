using System.Security.Claims;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.JWT.Claims;

public abstract class JwtClaimBuilderBase<TBuilder> where TBuilder : JwtClaimBuilderBase<TBuilder>
{
    private readonly List<Claim> claims = [];
    public IReadOnlyCollection<Claim> Build() => claims.AsReadOnly();
    protected TBuilder Add(string type, string value)
    {
        claims.Add(new Claim(type, value));
        return (TBuilder)this;
    }

    protected TBuilder Add(string type, DateTimeOffset value)
    {
        return Add(type, value.ToUnixTimeSeconds().ToString());
    }

    protected TBuilder Add(string type, bool value)
    {
        return Add(type, value.ToString());
    }

    public TBuilder WithAudience(string audience) => Add(AppClaimTypes.Aud, audience);
    public TBuilder WithIssuer(string issuer) => Add(AppClaimTypes.Iss, issuer);
    public TBuilder WithSubject(string subject) => Add(AppClaimTypes.Sub, subject);
    public TBuilder WithTokenId(string tokenId) => Add(AppClaimTypes.Jti, tokenId);
    public TBuilder WithNonce(string nonce) => Add(AppClaimTypes.Nonce, nonce);
    public TBuilder WithIssuedTime(DateTimeOffset time) => Add(AppClaimTypes.Iat, time);
    public TBuilder WithExpirationTime(DateTimeOffset time) => Add(AppClaimTypes.Exp, time);
}
